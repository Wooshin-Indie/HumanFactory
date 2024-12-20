using HumanFactory.Manager;
using UnityEngine;

namespace HumanFactory.Controller
{
    public class HumanController : MonoBehaviour
    {

        [Header("Movement Args")]
        [SerializeField] private float moveSpeed;       // TODO - 1 grid per 1 sec

        [Header("Debug")]
        [SerializeField] private Vector2Int currentPos; // 현재 그리드 위치
        [SerializeField] private Vector2Int targetPos;  // 이동할 그리드 위치
        [SerializeField] private int currentDir;        // 이동할 방향
        [SerializeField] private HumanOperandType operandType;

        public Vector2Int CurrentPos { get => currentPos; set => currentPos = value; }
        public Vector2Int TargetPos { get => targetPos; }

        public void UpdateCurpos()
        {
            currentPos = targetPos;
        }

        private int operandsResult = 0;
        public void SetAsOperand1() {
            operandType = HumanOperandType.Operand1;
            operandsResult = 0;    // TODO - 내가 들고있는 값으로 바꿔야함
        }

        public void SetOperands(int number) {
            operandsResult += number;
        }

        public int SetAsOperand2() {
            // TODO - 내가 들고있는 값 리턴해야됨
            // 애니메이션 필요하면 애니메이션 추가해야됨
            operandType = HumanOperandType.Operand2;
            return -1;
        }

        public void ExecuteOperand()
        {
            switch (operandType) {
                case HumanOperandType.Operand1:
                    // TODO - 현재 들고있는 값을 operandResult 값으로 바꾸기
                    break;
                case HumanOperandType.Operand2:
                    Destroy(this.gameObject);       //HACK - Destory가 아니라 좀 더 일찍 스르륵 사라지게 할 수도
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

        //private void Start()
        //{
        //    statemachine = new playerstatemachine();
        //    idlestate = new idlestate(this, statemachine);
        //    statemachine.init(idlestate);
        //}

        //private void update()
        //{
        //    statemachine.curstate.handleinput();

        //    statemachine.curstate.logicupdate();
        //}

        //private void fixedupdate()
        //{
        //    statemachine.curstate.physicsupdate();
        //}


        private void Awake()
        {

            // HACK - must set appropriate pos after instantiate
            currentPos = new Vector2Int(0, -1);
            targetPos = new Vector2Int(0, 0);
            currentDir = 0;
        }

        // 
        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                new Vector3(targetPos.x, targetPos.y, Constants.HUMAN_POS_Z),
                moveSpeed * Time.deltaTime);

            //UpdateTargetPos();
        }

        /// <summary>
        /// Update targetPos when transform.position is nearby it
        /// </summary>
        private void UpdateTargetPos()
        {
            float eulerDistance = Mathf.Abs(transform.position.x - targetPos.x) + Mathf.Abs(transform.position.y - targetPos.y);

            if (eulerDistance > Mathf.Epsilon) return;

            Debug.Log("UPDATE TARGET POSITION");

            // 점하고 가까울때만 실행
            transform.position = new Vector3(targetPos.x, targetPos.y, Constants.HUMAN_POS_Z);
            currentPos = targetPos;

            MapGrid grid = MapManager.Instance.ProgramMap[currentPos.x, currentPos.y];

            //
            grid.GetPadParameter(out currentDir);

            targetPos += new Vector2Int(Constants.DIR_X[currentDir], Constants.DIR_Y[currentDir]);
            if(targetPos.x >= MapManager.Instance.ProgramMap.GetLength(0) || targetPos.y >= MapManager.Instance.ProgramMap.GetLength(1)
                || targetPos.x < 0 || targetPos.y < 0)
            {
                // TODO - Disappear. It could cause err
                Destroy(gameObject);
            }

            Debug.Log("TARGET POS : " + TargetPos);

        }
    }
}