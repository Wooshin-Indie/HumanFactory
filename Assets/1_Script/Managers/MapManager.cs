using HumanFactory.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering.Universal;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.Managers
{

    public class MapGrid
    {
        private GridType type = 0;
        private PadType padType = 0;
        private BuildingType buildingType = 0;
        private ButtonInfos buttonInfo = new ButtonInfos(null);

        public GridType Type {  get => type; set => type = value; }
        public PadType PadType { get => padType; set => padType = value; }
        public BuildingType BuildingType { get => buildingType; set => buildingType = value; }
        public ButtonInfos ButtonInfo { get=>buttonInfo; }

        private bool isBlocked = false;
        private bool isPressed = false;

        public bool IsBlocked { get => isBlocked; set => isBlocked = value; }
        public bool IsPressed { get => isPressed; set => isPressed = value; }


        private PadType originPadType = 0;

        public MapGrid()
        {
            type = GridType.Empty;
        }

        public void GetPadParameter(out int dir)
        {
            dir = (int)padType;
        }

        public void OnRelease()
        {
            // TODO - 버튼이 있으면 동작 수행
            if (!isPressed) return;
            isPressed = false;
        }

        public void OnPressed()
        {
            // TODO - 버튼이 있으면 동작 수행
            if (isPressed) {
                Debug.LogError("Logic Err : MapGrid - Pressed Twice");
                return;
            }
            isPressed = true;
        }

        public void OnButtonRotate(PadType type)
        {
            // 버튼 누르면 바닥 회전
            originPadType = padType;
            padType = type;
        }

        public void OnButtonStop()
        {
            padType = originPadType;
        }

        public void OffButtonRotate()
        {
            isBlocked = false;
            // TODO - 이게 버튼이면 연결된 애들도 original로 변경해야됨
        }

        public void OffButtonStop()
        {
            isBlocked = false;
        }
    }

    public class MapManager : MonoBehaviour
    {
        #region Singleton
        private static MapManager instance;
        public static MapManager Instance { get { return instance; } }

        private void Init()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
        }
        #endregion

        private void Awake()
        {
            Init();
        }

        private MapGrid[,] programMap;
        public MapGrid[,] ProgramMap { get => programMap; }

        [SerializeField] private Vector2Int mapSize;

        private void Start()
        {
            programMap = new MapGrid[mapSize.x, mapSize.y];
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    programMap[i, j] = new MapGrid();
                }
            }

            // HACK - Temproary testing Pad (direction change)
            programMap[2, 0].Type = GridType.Pad;
            programMap[2, 0].PadType = PadType.DirRight;
            programMap[4, 0].Type = GridType.Pad;
            programMap[4, 0].PadType = PadType.DirLeft;
            programMap[0, 1].Type = GridType.Pad;
            programMap[0, 1].PadType = PadType.DirRight;
            programMap[4, 1].Type = GridType.Pad;
            programMap[4, 1].PadType = PadType.DirUp;
            programMap[2, 2].Type = GridType.Pad;
            programMap[2, 2].PadType = PadType.DirDown;
            programMap[4, 2].Type = GridType.Pad;
            programMap[4, 2].PadType = PadType.DirLeft;
            programMap[0, 4].Type = GridType.Pad;
            programMap[4, 4].Type = GridType.Pad;


            secFuncs.Add(instance.ExecuteAtOneThird);
            secFuncs.Add(instance.ExecuteAtHalfTime);
            secFuncs.Add(instance.ExecuteAtTwoThirds);
        }

        private bool isCycleRunning = false;
        private void Update()
        {
            if (!isCycleRunning)
                StartCoroutine(ProgramCycleCoroutine());
        }

        [Header("Game Cycle")]
        [SerializeField, Range(0.5f, 2.0f)] private float cycleTime;
        
        private List<HumanController> humanControllers = new List<HumanController>();

        private List<bool> flags = new List<bool> { false, false, false };
        private List<float> timeSections = new List<float> { 0.3f, 0.5f, 0.7f };
        private List<Func<bool>> secFuncs = new List<Func<bool>>();
        private float cycleElapsedTime = 0f;

        private IEnumerator ProgramCycleCoroutine()
        {
            InitPerCycle();
            
            while (cycleElapsedTime < cycleTime)
            {
                ExecutePerFrame(cycleElapsedTime, cycleTime);
                yield return null;
                cycleElapsedTime += Time.deltaTime;
            }

            FinPerCycle();
        }

        /// <summary>
        /// 싸이클 시작할 떄 수행해야하는 애들
        /// 변수 값 초기화가 주 목적
        /// </summary>
        private void InitPerCycle()
        {
            Debug.Log("싸이클 시작!");
            isCycleRunning = true;
            cycleElapsedTime = 0f;
            for (int i = 0; i < flags.Count; i++)
            {
                flags[i] = false;
            }

        }

        /// <summary>
        /// 매 프레임 실행하는 함수
        /// </summary>
        private void ExecutePerFrame(float elapsedTime, float maxTime)
        {
            for (int i = 0; i < flags.Count; i++)
            {
                if (flags[i] || !(cycleElapsedTime > cycleTime * timeSections[i])) continue;
                flags[i] = secFuncs[i].Invoke();
            }

            foreach(var controller in humanControllers)
            {
                // TODO - 매 프레임 Lerp로 Player 이동하는 코드 필요
            }
        }

        /// <summary>
        /// 1/3 경과 시, Button을 전부 Release하여 맵을 Original로 만듦
        /// </summary>
        private bool ExecuteAtOneThird()
        {

            Debug.Log("싸이클 0.3");
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    programMap[i, j].OnRelease();
                }
            }
            return true;
        }

        // HashSet에 위치를 전부넣어서 TargetPos가 겹치는 애들을 찾음
        private Dictionary<Vector2Int, List<HumanController>> targetPosSet = new Dictionary<Vector2Int, List<HumanController>>();
        private Vector2Int tmpPos;

        /// <summary>
        /// 1/2 경과 시, 플레이어의 Vector2Int CurPos를 업데이트 시킴
        /// CurPos가 겹치는 경우를 확인하고 겹치는 경우에 더할 준비를 해야됨
        /// controller에 변수를 하나두고 체크해서 그 경우엔 곧 사라지도록 or 더해지도록
        /// </summary>
        private bool ExecuteAtHalfTime()
        {

            Debug.Log("싸이클 0.5");
            // Dictionary 에 같은 좌표로 이동하는 애들끼리 모음
            foreach (var controller in humanControllers)
            {
                controller.UpdateCurpos();
                tmpPos = controller.CurrentPos;
                if (!targetPosSet.ContainsKey(tmpPos))
                {
                    targetPosSet[tmpPos] = new List<HumanController>();
                }
                targetPosSet[tmpPos].Add(controller);
            }

            // 같은 곳에 겹치는 애들만 모음
            IEnumerable<List<HumanController>> tmpList = targetPosSet
                .Where(item => item.Value.Count >= 2)
                .Select(item => item.Value);

            foreach (var list in tmpList)
            {
                list[0].SetAsOperand1();
                for (int i = 1; i < list.Count; i++)
                {
                    list[0].SetOperands(list[i].SetAsOperand2());
                }
            }

            return true;
        }

        /// <summary>
        /// 2/3 경과 시, 이동한 위치의 Button을 Press 하고 변경사항 업데이트
        /// </summary>
        private bool ExecuteAtTwoThirds()
        {
            Debug.Log("싸이클 0.7");
            foreach (var controller in humanControllers)
            {
                programMap[controller.CurrentPos.x, controller.CurrentPos.y].OnPressed();
            }
            return true;
        }

        /// <summary>
        /// 전부 경과하여 이동 완료
        /// 1/2 에서 controller에 체크한 걸로 더하기 연산 먼저 수행
        /// 다음 이동할 위치 설정, 연산 수행(+1, +/- ...)
        /// </summary>
        private void FinPerCycle()
        {
            isCycleRunning = false;
            foreach (var controller in humanControllers)
            {
                controller.ExecuteOperand();
                controller.OnFinPerCycle();
            }

            Debug.Log("싸이클 끝!");
        }
    }
}