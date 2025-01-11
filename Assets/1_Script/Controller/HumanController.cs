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
        public Vector2Int TargetPos { get => targetPos; }
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
                    Destroy(gameObject);       //HACK - Destory가 아니라 좀 더 일찍 스르륵 사라지게 할 수도
                    break;
            }
        }

        private void UnsetOperand() { 
            operandType = HumanOperandType.None;
        }

        public void OnFinPerCycle()
        {
            UpdateTargetPos();
            UnsetOperand();
        }


        private void Awake()
        {
            // HACK - must set appropriate pos after instantiate
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

        /// <summary>
        /// Update targetPos when transform.position is nearby it
        /// </summary>
        private void UpdateTargetPos()
        {
            float eulerDistance = Mathf.Abs(transform.position.x - targetPos.x) + Mathf.Abs(transform.position.y - targetPos.y);


            // 점하고 가까울때만 실행
            transform.position = new Vector3(targetPos.x, targetPos.y, Constants.HUMAN_POS_Z);
            currentPos = targetPos;

            MapGrid grid = MapManager.Instance.ProgramMap[currentPos.x, currentPos.y];

            //
            grid.GetPadParameter(out currentDir);

			prevPos = targetPos;
			targetPos += new Vector2Int(Constants.DIR_X[currentDir], Constants.DIR_Y[currentDir]);

        }

        public void EffectTestFunc(EffectType type)
        {
            Managers.Effect.ShowSpriteEffect(transform.position + new Vector3(0, 0.2f, 0),
                type);
        }

        public void HumanDyingProcess()
        {
            transform.DOMove(new Vector3(targetPos.x, targetPos.y, Constants.HUMAN_POS_Z), MapManager.Instance.CycleTime);
            GetComponent<SpriteRenderer>().DOFade(0, MapManager.Instance.CycleTime).
                        OnComplete(() =>
                        {
                            Destroy(this.gameObject);
                        });
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