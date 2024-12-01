using HumanFactory.Managers;
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

            UpdateTargetPos();
        }

        /// <summary>
        /// Update targetPos when transform.position is nearby it
        /// </summary>
        private void UpdateTargetPos()
        {
            float eulerDistance = Mathf.Abs(transform.position.x - targetPos.x) + Mathf.Abs(transform.position.y - targetPos.y);

            if (eulerDistance > Mathf.Epsilon) return;

            // 점하고 가까울때만 실행
            transform.position = new Vector3(targetPos.x, targetPos.y, Constants.HUMAN_POS_Z);
            currentPos = targetPos;

            MapGrid grid = MapManager.Instance.ProgramMap[currentPos.x, currentPos.y];

            switch (grid.Type) {
                case GridType.Empty:
                    break;
                case GridType.Pad:
                    grid.GetPadParameter(out currentDir);
                    break;
            }

            targetPos += new Vector2Int(Constants.DIR_X[currentDir], Constants.DIR_Y[currentDir]);
            if(targetPos.x >= MapManager.Instance.ProgramMap.GetLength(0) || targetPos.y >= MapManager.Instance.ProgramMap.GetLength(1)
                || targetPos.x < 0 || targetPos.y < 0)
            {
                // TODO - Disappear. It could cause err
                Destroy(gameObject);
            }
        }

        // 해야할 일
        // 1. 기획을 탄탄하게 - 다양한 퍼즐이 가능하게 구상. 어셈블리어 -> branch jump -> 이걸 어떻게 시각적으로 나타내지?
        // 어떤 애들이 있어야 재밌을지 ( Jump, Branch, for, ... )
        // 2. StateMachine - Finite Statemachine 이라고 구글에 치면 나오고... -> 애니메이션 관리 편함 -> 이거로 바꾸기
        // 3. 디자인 결정 - 컨셉과 스프라이트를 구해야됨
    }
}

// 한글 주석 테스트 입니다.