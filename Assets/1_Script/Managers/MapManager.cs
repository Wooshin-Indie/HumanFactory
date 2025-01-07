using DG.Tweening;
using HumanFactory.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

            SetPad(padType);
        }

        public void SetPad(PadType type)
		{
            PadType = type;
			switch (type)
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
            {
                buildingSprite.sprite = Managers.Resource.GetBuildingSprite(type, false);
                buildingSprite.color = Color.white;
            }
            else
                buildingSprite.sprite = null;
        }

        public void PreviewBuilding(BuildingType type)
        {
            if (type != BuildingType.None)
            {
                buildingSprite.sprite = Managers.Resource.GetBuildingSprite(type, false);
                buildingSprite.color = Constants.COLOR_INVISIBLE;
            } 
        }

        public void UnpreviewBuilding()
        {
            if (buildingType != BuildingType.None) return;

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
                // 여러 사람이 들어올수도 있음
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

        public StageGridData GetStageGridData()
        {
            StageGridData data = new StageGridData();
            data.posX = posX;
            data.posY = posY;
            data.padtype = padType;
            data.buildingType = buildingType;
            data.buttonInfos = new ButtonInfos(buttonInfo);

            return data;
        }

        public void SetStageGridInfo(StageGridData data)
		{
			posX = data.posX;
			posY = data.posY;
			SetPad(data.padtype);
			SetBuilding(data.buildingType);
			buttonInfo = new ButtonInfos(data.buttonInfos);
		}

        public void ClearGrid()
		{
			SetPad(PadType.DirNone);
            SetBuilding(BuildingType.None);
            buttonInfo = new ButtonInfos(new Vector2Int(-1, -1));
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
        private StageInfo currentStageInfo;
        public StageInfo CurrentStageInfo { get => currentStageInfo; }
        private MapGrid[,] programMap;
        public MapGrid[,] ProgramMap { get => programMap; }

        [SerializeField] private Vector2Int mapSize;
        public Vector2Int MapSize { get => mapSize; }
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private GameObject humanPrefab;

        [Header("Circuit")]
        [SerializeField] private SpriteRenderer buttonRect;
        [SerializeField] private SpriteRenderer tileRect;

        private void Start()
        {
            buttonRect.gameObject.SetActive(false);
            tileRect.gameObject.SetActive(false);
            currentStageInfo = Managers.Resource.GetStageInfo(0);

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

            //humanControllers.Add(Instantiate(humanPrefab, new Vector3(0f, -1f, Constants.HUMAN_POS_Z), Quaternion.identity)
            //    .GetComponent<HumanController>());
        }

        private bool isCycleRunning = false;
        private bool isCircuiting = false;
        private Vector2Int circuitingButtonPos;
        public Vector2Int prevHoverPos = new Vector2Int(0, 0);
        public bool IsCircuiting { get => isCircuiting; }

		#region CycleLock
		private int cycleLock = 0;
		public void LockCycle()
		{
			cycleLock++;
		}
		public void ReleaseCycle()
		{
			if (cycleLock > 0)
				cycleLock--;
		}
		private bool IsCycleEnabled()
		{
			return cycleLock <= 0;
		}

		#endregion
		private void Update()
        {
            if (!isCycleRunning)
            {
                if (!IsCycleEnabled()) return;
                StartCoroutine(ProgramCycleCoroutine());
            }

            // HACK : 저장 타이밍 따로 정해줘야됨
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveStage();
            }

        }

		#region Screen Interact

		public MapGrid GetMapGrid(int x, int y)
        {
            return programMap[x, y];
        }
        public bool CheckBoundary(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y);
        }
        public void OnHoverMapGridInNoneMode(int x, int y)
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

		private Vector2Int prevDirPad = new Vector2Int(-1, -1);
		public void OnClickMapGridInPadMode(int x, int y)
        {
            if (prevDirPad.x == x && prevDirPad.y == y) return;
            
            if(!CheckBoundary(prevDirPad.x, prevDirPad.y))
			{
				if (CheckBoundary(x, y))
				{
					programMap[x, y].OnClickRotate();
					prevDirPad.Set(x, y);
					return;
				}
                else
                {
                    return;
                }
			}

			Vector2Int dir = new Vector2Int(x, y) - prevDirPad;

            PadType type = PadType.DirNone;
            if (dir.x == 1)
            {
                type = PadType.DirRight;
            }
            else if (dir.x == -1)
			{
				type = PadType.DirLeft;
			}
            else if (dir.y == 1)
			{
				type = PadType.DirUp;
			}
            else if (dir.y == -1)
			{
				type = PadType.DirDown;
			}

            programMap[prevDirPad.x, prevDirPad.y].SetPad(type);
            if (CheckBoundary(x, y)) programMap[x, y].SetPad(type);

			prevDirPad.Set(x, y);
		}
        public void OnReleaseMapGridInPadMode()
		{
            prevDirPad = new Vector2Int(-1, -1);
	    }

        public void OnRightClickMapGridInPadMode(int x, int y)
		{
            if (!CheckBoundary(x, y)) return;

            programMap[x, y].SetPad(PadType.DirNone);
		}

		public void OnHoverMapGridInBuildingMode(int x, int y, BuildingType type)
        {
            if (!CheckBoundary(x, y))
            {
                programMap[prevHoverPos.x, prevHoverPos.y].UnpreviewBuilding();
                return;
            }
            if (prevHoverPos.x == x && prevHoverPos.y == y) return;

            if (programMap[x, y].BuildingType == BuildingType.None)
            {
                programMap[prevHoverPos.x, prevHoverPos.y].UnpreviewBuilding();
                programMap[x, y].PreviewBuilding(type);
                prevHoverPos.Set(x, y);
            }

        }
        public void OnClickMapGridInBuildingMode(int x, int y, BuildingType type)
        {
            if (!CheckBoundary(x, y)) return;
            programMap[x, y].SetBuilding(type);
        }

        public void OnInputModeChanged(InputMode mode)
        {
            programMap[prevHoverPos.x, prevHoverPos.y].UnpreviewBuilding();
            buttonRect.gameObject.SetActive(false);
            tileRect.gameObject.SetActive(false);
            prevDirPad.Set(-1, -1);
			ChangeMapVisibility(mode);
        }
		#endregion

		#region Game Cycle

		[Header("Game Cycle")]
        [SerializeField, Range(0.5f, 2.0f)] private float cycleTime;
        
        // TODO - Destory하고 null값 남아있음 처리 필요.
        [SerializeField]private List<HumanController> humanControllers = new List<HumanController>();

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


        private bool isPersonAdd = false;


        public void DoubleCycleTime()
        {
            cycleTime = 0.5f;
        }
        public void AddPerson()
        {
            cycleTime = 1f;
            isPersonAdd = true;
        }

        [ContextMenu("Stop Add")]
        public void StopAddPerson()
        {
            isPersonAdd = false;
        }

        /// <summary>
        /// 싸이클 시작할 떄 수행해야하는 애들
        /// 변수 값 초기화가 주 목적
        /// </summary>
        private int idxin = 0; //테스트용
        private void InitPerCycle()
        {
            isCycleRunning = true;

            if (isPersonAdd && idxin < currentStageInfo.inputs.Length )
            {
                HumanController tmpController = Instantiate(humanPrefab, new Vector3(0f, -1f, Constants.HUMAN_POS_Z), Quaternion.identity)
                    .GetComponent<HumanController>();
                humanControllers.Add(tmpController);

                tmpController.HumanNum = currentStageInfo.inputs[idxin];
                Debug.Log(currentStageInfo.inputs[idxin]);
                idxin++;
            }



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
                controller.SetPositionByRatio(elapsedTime/maxTime);
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

        private int idxout = 0;
        private bool isOutputCorrect = true;
        /// <summary>
        /// 전부 경과하여 이동 완료
        /// 1/2 에서 controller에 체크한 걸로 더하기 연산 먼저 수행
        /// 다음 이동할 위치 설정, 연산 수행(+1, +/- ...)
        /// </summary>
        private void FinPerCycle()
        {
            foreach (var controller in humanControllers)
            {
                controller.ExecuteOperand();
                // TODO - 맵과 상호작용하여 연산해야됨
            }

            humanControllers.RemoveAll(item => item.OperandType == HumanOperandType.Operand2);


            /*
             * 1. 아도겐 코드임
             * 
             * > for, if 같이 depth가 늘어나는 경우에는 depth가 너무 깊어지지 않도록 주의해야됨
             *   예를 들어, if(cond){~} 가 아니라 if(!coand) continue; ~ 같이 바꿀 수 있음
             *   
             *   
             * 2. 바로 위에 RemoveAll이 있는 이유는 Destroy 된 애들을 전부 없애기 위함임
             *    걔네를 List에서 없애지않고 접근하면 에러가 남
             *    
             *    Output은 항상 하나만 나오므로 (1) 찾고, (2) Destroy하고 (3) Erase하고 끝
             *    break로 빠져나와야함 
             */

            for (int i = humanControllers.Count - 1; i >= 0; i--)
            {
                humanControllers[i].OnFinPerCycle();

                if (!(humanControllers[i].CurrentPos.x == mapSize.x - 1 && humanControllers[i].CurrentPos.y == mapSize.y - 1)) continue;
                //human이 output지점 (4,4)가 아니면 스킵

                if (idxout < currentStageInfo.outputs.Length)
                {
                    if (humanControllers[i].HumanNum != currentStageInfo.outputs[idxout])
                    {
                        isOutputCorrect = false;
                    }
                    idxout++;
                }
                Destroy(humanControllers[i].gameObject);
                humanControllers.Remove(humanControllers[i]);
            }

            if (idxout >= currentStageInfo.outputs.Length)
            {
                Debug.Log("OUTPUT : " + isOutputCorrect);
                idxout = 0;
                isOutputCorrect = true;
            }

            isCycleRunning = false;
        }

		#endregion

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

        #region Save/Load Data

        private int currentStage = -1;

        public int CurrentStage { get => currentStage; }
        


        public void LoadStage(int stageId)
        {
            Debug.Log($"Load Stage : {stageId}");

            currentStage = stageId;
            currentStageInfo = Managers.Resource.GetStageInfo(stageId);

			// TODO : StageInfo에 따라 Map Width라던지 전부 setting 해야됨

			for (int i = 0; i < mapSize.x; i++)
			{
				for (int j = 0; j < mapSize.y; j++)
				{
                    programMap[i, j].ClearGrid();
				}
			}


			StageGridDatas tmpGridData = Managers.Data.GetGridDatas(stageId);
            if (tmpGridData == null) return;

            foreach (var data in tmpGridData.gridDatas)
            {
                programMap[data.posX, data.posY].SetStageGridInfo(data);
            }

        }

        public void SaveStage()
		{
			Debug.Log($"Save Stage : {currentStage}");

			StageGridDatas gridDatas = new StageGridDatas();

            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    gridDatas.gridDatas.Add(programMap[i, j].GetStageGridData());
                }
            }

            Managers.Data.AddStageGridData(currentStage, gridDatas);
        }

		#endregion

	}
}