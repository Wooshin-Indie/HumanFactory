using DG.Tweening;
using HumanFactory.Controller;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HumanFactory.Manager
{

    public class MapGrid
    {
        private int posX, posY;
        private SpriteRenderer arrowSprite;
        private SpriteRenderer buildingSprite;
        private PadType padType = PadType.DirNone;
        private BuildingType buildingType = BuildingType.None;
        private ButtonInfos buttonInfo = new ButtonInfos(new Vector2Int(-1, -1));

        public int PosX { get => posX; }
        public int PosY { get => posY; }
        public PadType PadType { get => padType; set => padType = value; }
        public BuildingType BuildingType { get => buildingType; set => buildingType = value; }
        public ButtonInfos ButtonInfo { get=>buttonInfo; }

        public Sprite BuildingSprite { get => buildingSprite.sprite; }

        private bool isBlocked = false;
        private bool isPressed = false;

        public bool IsBlocked { get => isBlocked; set => isBlocked = value; }
        public bool IsPressed { get => isPressed; set => isPressed = value; }


        private PadType originPadType = 0;

        public MapGrid(int posX, int posY, SpriteRenderer arrow, SpriteRenderer building)
        {
            this.posX = posX;
            this.posY = posY;
            arrowSprite = arrow;
            buildingSprite = building;
            padType = PadType.DirNone;
            arrow.color = Constants.COLOR_TRANS;
            building.sprite = null;
            building.color = Color.white;
        }

        public void GetPadParameter(out int dir)
        {
            dir = (int)padType;
        }

        public void OnClickRotate()
        {
            padType = (PadType)(((int)padType + 1) % Enum.GetValues(typeof(PadType)).Length);

            switch (padType)
            {
                case PadType.DirLeft:
                case PadType.DirRight:
                case PadType.DirUp:
                case PadType.DirDown:
                    arrowSprite.color = Constants.COLOR_ARROW;
                    arrowSprite.transform.rotation = Quaternion.Euler(0f, 0f, -90f * (int)padType);
                    break;
                case PadType.DirNone:
                    arrowSprite.color = Constants.COLOR_TRANS;
                    break;
            }
        }

        public void SetBuilding(BuildingType type)
        {
            buildingType = type;
            if (type != BuildingType.None)
                buildingSprite.sprite = Managers.Resource.GetBuildingSprite(type, false);
            else
                buildingSprite.sprite = null;
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

        public void OffButtonRotate()
        {
            padType = originPadType;
            // TODO - 이게 버튼이면 연결된 애들도 original로 변경해야됨
        }
        public void OnButtonStop()
        {
            isBlocked = false;
        }


        public void OffButtonStop()
        {
            isBlocked = false;
        }

        public void SetVisibility(InputMode mode)
        {
            float duration = 0.5f;
            switch (mode) { 
                case InputMode.None:
                    if(padType != PadType.DirNone) arrowSprite.DOColor(Constants.COLOR_ARROW, duration);
                    buildingSprite.DOColor(Constants.COLOR_WHITE, duration);
                    break;
                case InputMode.Pad:
                    if (padType != PadType.DirNone) arrowSprite.DOColor(Constants.COLOR_ARROW, duration);
                    buildingSprite.DOColor(Constants.COLOR_INVISIBLE, duration);
                    break;
                case InputMode.Building:
                    if (padType != PadType.DirNone) arrowSprite.DOColor(Constants.COLOR_INVISIBLE, duration);
                    buildingSprite.DOColor(Constants.COLOR_WHITE, duration);
                    break;
            }
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
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private GameObject humanPrefab;

        [Header("Circuit")]
        [SerializeField] private SpriteRenderer buttonRect;
        [SerializeField] private SpriteRenderer tileRect;

        private void Start()
        {
            buttonRect.gameObject.SetActive(false);
            tileRect.gameObject.SetActive(false);

            programMap = new MapGrid[mapSize.x, mapSize.y];
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    programMap[i, j] = new MapGrid(i, j, 
                        Instantiate(arrowPrefab, new Vector3(i, j, 0f), Quaternion.identity).GetComponent<SpriteRenderer>(),
                        Instantiate(arrowPrefab, new Vector3(i, j, 0f), Quaternion.identity).GetComponent<SpriteRenderer>()
                        );
                }
            }

            secFuncs.Add(instance.ExecuteAtOneThird);
            secFuncs.Add(instance.ExecuteAtHalfTime);
            secFuncs.Add(instance.ExecuteAtTwoThirds);

            humanControllers.Add(Instantiate(humanPrefab, new Vector3(0f, -1f, Constants.HUMAN_POS_Z), Quaternion.identity)
                .GetComponent<HumanController>());
        }

        private bool isCycleRunning = false;
        private bool isCircuiting = false;
        private Vector2Int circuitingButtonPos;
        public bool IsCircuiting { get => isCircuiting; }
        private void Update()
        {
            if (!isCycleRunning)
                StartCoroutine(ProgramCycleCoroutine());
        }

        public MapGrid GetMapGrid(int x, int y)
        {
            return programMap[x, y];
        }
        public bool CheckBoundary(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y);
        }
        public void OnHoverMapGrid(int x, int y)
        {
            if (isCircuiting)
            {
                
                buttonRect.transform.position = new Vector3(circuitingButtonPos.x,
                    circuitingButtonPos.y, Constants.HUMAN_POS_Z);
                tileRect.transform.position = new Vector3(x, y, Constants.HUMAN_POS_Z);
                if (circuitingButtonPos.x != x || circuitingButtonPos.y != y)
                    tileRect.gameObject.SetActive(true);
                else
                    tileRect.gameObject.SetActive(false);

                return;
            }

            if (!CheckBoundary(x, y) || programMap[x, y].BuildingType == BuildingType.None)
            {
                buttonRect.gameObject.SetActive(false);
                tileRect.gameObject.SetActive(false);
            }
            else
            {
                buttonRect.transform.position = new Vector3(x, y, Constants.HUMAN_POS_Z);
                buttonRect.sprite = programMap[x, y].BuildingSprite;
                buttonRect.gameObject.SetActive(true);
                
                if (programMap[x, y].BuildingType != BuildingType.Button
                && programMap[x, y].BuildingType != BuildingType.Jump)
                {
                    tileRect.transform.position = new Vector3(x, y, Constants.HUMAN_POS_Z);
                    tileRect.gameObject.SetActive(true);
                }
                else if (programMap[x, y].ButtonInfo.linkedGridPos.x < 0)
                {
                    tileRect.gameObject.SetActive(false);
                }
                else
                {
                    tileRect.transform.position = new Vector3(programMap[x, y].ButtonInfo.linkedGridPos.x,
                        programMap[x, y].ButtonInfo.linkedGridPos.y,
                        Constants.HUMAN_POS_Z);
                    tileRect.gameObject.SetActive(true);
                }
            }
        }


        public void OnClickMapGridInNoneMode(int x, int y, bool isSet)
        {
            if (!CheckBoundary(x, y)) return;

            if (isCircuiting == isSet) return;

            if (isCircuiting)   // 회로작업 중이면 -> 클릭했을 때 전에 클릭했던 버튼과 연결 
            {
                if (circuitingButtonPos.x == x && circuitingButtonPos.y == y) return;

                programMap[circuitingButtonPos.x, circuitingButtonPos.y].ButtonInfo.linkedGridPos 
                    = new Vector2Int(x, y);
                isCircuiting = false;
            }
            else
            {
                // TODO - UI 띄우기
                if (programMap[x, y].BuildingType != BuildingType.Jump
                    && programMap[x, y].BuildingType != BuildingType.Button) return;

                // HACK : 임시로 circuiting 시험용 코드입니다.
                isCircuiting = true;
                circuitingButtonPos = new Vector2Int(x, y);
            }

            //isCircuiting = isSet;
        }

        public void OnClickMapGrid(int x, int y)
        {
            if (!CheckBoundary(x, y)) return;
            programMap[x, y].OnClickRotate();
        }
        public void OnClickMapGridInBuildingMode(int x, int y, BuildingType type)
        {
            if (!CheckBoundary(x, y)) return;
            programMap[x, y].SetBuilding(type);
        }

        public void OnInputModeChanged(InputMode mode)
        {
            buttonRect.gameObject.SetActive(false);
            tileRect.gameObject.SetActive(false);
            ChangeMapVisibility(mode);
        }

        [Header("Game Cycle")]
        [SerializeField, Range(0.5f, 2.0f)] private float cycleTime;
        
        // TODO - Destory하고 null값 남아있음 처리 필요.
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
                // Action에 대해서 이친구는 리턴값이 없는 함수를 저장하는 자료형
                // bool return 을 해야해 -> Func<T1, T2, T3> 
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
            targetPosSet.Clear();

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
                // TODO - 맵과 상호작용하여 연산해야됨
                controller.OnFinPerCycle();
            }
        }

        private void ChangeMapVisibility(InputMode mode)
        {
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    programMap[i, j].SetVisibility(mode);
                }
            }
        }
    }
}