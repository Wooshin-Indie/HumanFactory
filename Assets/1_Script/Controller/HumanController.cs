using DG.Tweening;
using HumanFactory.Manager;
using HumanFactory.Util;
using TMPro;
using UnityEngine;

namespace HumanFactory.Controller
{
    public class HumanController : MonoBehaviour
    {

        [Header("Debug")]
        [SerializeField] private Vector2Int currentPos; // 현재 그리드 위치
        [SerializeField] private Vector2Int targetPos;  // 이동할 그리드 위치
        [SerializeField] private Vector2Int prevPos;  // 이전 그리드 위치
        [SerializeField] private int currentDir;        // 이동할 방향
        [SerializeField] private HumanOperandType operandType;
        [SerializeField] private int humanNum = 2;

        [SerializeField] private TextMeshPro numTMP;

        public Vector2Int CurrentPos { get => currentPos; set => currentPos = value; }
        public Vector2Int PrevPos { get => prevPos; set => prevPos = value; }
        public Vector2Int TargetPos { get => targetPos; set => targetPos = value; }
        public int HumanNum { get => humanNum; 
            set
            {
                humanNum = value;
                numTMP.text = humanNum.ToString();
            }
        }

        public HumanOperandType OperandType { get => operandType; }

        public void UpdateCurpos()
        {
            currentPos = targetPos;
        }

        private int operandsResult = 0;
        public void SetAsOperand1() {
            operandType = HumanOperandType.Operand1;
            operandsResult = humanNum;
        }

        public void SetOperands(int number) {
            operandsResult += number;
        }



        public int SetAsOperand2() {
            // 애니메이션 필요하면 애니메이션 추가해야됨
            operandType = HumanOperandType.Operand2;
            return humanNum;
        }

        public void ExecuteOperand()
        {
            switch (operandType) {
                case HumanOperandType.Operand1:
                    HumanNum = operandsResult;
                    break;
                case HumanOperandType.Operand2:
                    Destroy(gameObject);
                    break;
            }
        }

        private void UnsetOperand() { 
            operandType = HumanOperandType.None;
        }

        public void OnInitPerCycle()
		{
			// Animation 속도 조절 (한 싸이클마다만 함)
			GetComponent<Animator>().speed = 1 / MapManager.Instance.CycleTime;
            
			if (prevPos == targetPos)
			{
				GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_IDLE);
			}
			else
			{
				GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_WALK);
			}

		}

        public void OnFinPerCycle()
		{
			UpdateTargetPos();

			UnsetOperand();
        }


        private void Awake()
        {
            prevPos = new Vector2Int(0, -1);
            currentPos = new Vector2Int(0, -1);
            targetPos = new Vector2Int(0, 0);
            currentDir = 0;
            numTMP.text = humanNum.ToString();
        }


        public void SetPositionByRatio(float ratio)
        {
            Vector2 tmpV = Vector2.Lerp(new Vector2(prevPos.x, prevPos.y), new Vector2(targetPos.x, targetPos.y), ratio);
            transform.position = new Vector3(tmpV.x, tmpV.y, Constants.HUMAN_POS_Z);
        }

		private bool isDoubled = false;
		public bool IsDoubled { get => isDoubled; }
		/// <summary>
		/// Update targetPos when transform.position is nearby it
		/// </summary>
		private void UpdateTargetPos()
        {
            if (isDoubled)
            {
                isDoubled = false;
                return;
            }

            // 점하고 가까울때만 실행
            transform.position = new Vector3(targetPos.x, targetPos.y, Constants.HUMAN_POS_Z);

            if (!MapManager.Instance.CheckBoundary(currentPos.x, currentPos.y)) return; //그리드 밖이면 return

            MapGrid grid = MapManager.Instance.ProgramMap[currentPos.x, currentPos.y];

            //
            grid.GetPadParameter(out currentDir);

            switch (currentDir) {
                case (int)PadType.DirLeft:
                    GetComponent<SpriteRenderer>().flipX = true;
                    break;
                case (int)PadType.DirRight:
					GetComponent<SpriteRenderer>().flipX = false;
					break;
            }


            prevPos = targetPos;
			targetPos += new Vector2Int(Constants.DIR_X[currentDir], Constants.DIR_Y[currentDir]);

        }

        public void SetAsDoubled(HumanController controller)
        {
            isDoubled = true;

            currentPos = controller.CurrentPos;
            targetPos = controller.CurrentPos;
            prevPos = controller.PrevPos;
            HumanNum = controller.HumanNum;
			MapManager.Instance.ProgramMap[currentPos.x, currentPos.y].GetPadParameter(out currentDir);

			Vector2Int dir = targetPos - prevPos;
            targetPos = targetPos + dir;
            prevPos = prevPos + dir;
        }

        public void EffectTestFunc(EffectType type)
        {
            Managers.Effect.ShowSpriteEffect(transform.position + new Vector3(0, 0.2f, 0),
                type);
        }

        public void HumanDyingProcess()
        {
            numTMP.gameObject.SetActive(false);

			GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_DIE);
            RuntimeAnimatorController rac = GetComponent<Animator>().runtimeAnimatorController;
            float duration = 0f;
            for (int i = 0; i < rac.animationClips.Length; i++)
            {
                if (rac.animationClips[i].name == "Human_Die")
                {
                    duration = rac.animationClips[i].length*MapManager.Instance.CycleTime;
                    break;
                }
            }

            GetComponent<SpriteRenderer>().DOFade(0f, duration).SetEase(Ease.InQuint).
                        OnComplete(() =>
                        {
                            Destroy(this.gameObject);
                        });
        }

        [SerializeField]
        private bool isTeleport = false;
        public bool IsTeleport { get => isTeleport; }
        public void OnTeleport()
        {
            isTeleport = true;
			targetPos = currentPos;
		}
        public void OffTeleport(Vector2Int vec)
        {
            isTeleport = false;
            currentPos = vec;
            targetPos = vec;
            transform.position.Set(vec.x, vec.y, Constants.HUMAN_POS_Z);
        }

		public void AddByButton()
        {
            HumanNum++;
			EffectTestFunc(EffectType.Addi);
        }

		public void SubByButton()
		{
			HumanNum--;
			EffectTestFunc(EffectType.Subi);
		}
	}
}