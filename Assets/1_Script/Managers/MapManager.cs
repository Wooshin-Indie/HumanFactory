using HumanFactory.Controller;
using JetBrains.Annotations;
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
        public ButtonInfos ButtonInfo { get => buttonInfo; set => buttonInfo = value; }

        public Sprite BuildingSprite { get => buildingSprite.sprite; }

        private bool isPressed = false;
        public bool IsPressed { get => isPressed; set => isPressed = value; }

        private bool isActive = true;
        public bool IsActive { get => isActive; set => isActive = value; }


        private PadType originPadType = 0;

        public MapGrid(int posX, int posY, SpriteRenderer arrow, SpriteRenderer building)
        {
            this.posX = posX;
            this.posY = posY;
            arrowSprite = arrow;
            buildingSprite = building;
            padType = PadType.DirNone;
            originPadType = padType;
            arrow.color = Constants.COLOR_TRANS;
            building.sprite = null;
            building.color = Color.white;
            buttonInfo.linkedGridPos.Set(posX, posY);
        }

        public void GetPadParameter(out int dir)
        {
            dir = (int)padType;
        }

        public void OnClickRotate()
        {
            padType = (PadType)(((int)padType + 1) % Enum.GetValues(typeof(PadType)).Length);
			originPadType = padType;
			SetPad(padType);
        }

        public void SetPad(PadType type, bool isPermanent = true)
		{
            padType = type;
            if(isPermanent) originPadType = padType;
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

        public void SetPadToOrigin()
        {
            SetPad(originPadType);
        }


		public void SetBuilding(BuildingType type)
        {
            buildingType = type;
            if (type != BuildingType.None)
            {
                isActive = true;
                buildingSprite.sprite = Managers.Resource.GetBuildingSprite(type, false, isActive);
                buildingSprite.color = Color.white;
            }
            else
                buildingSprite.sprite = null;
        }

        public void PreviewBuilding(BuildingType type)
        {
            if (type != BuildingType.None)
            {
                buildingSprite.sprite = Managers.Resource.GetBuildingSprite(type, false, true);
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
            if (!isPressed) return;
            isPressed = false;
			buildingSprite.sprite = Managers.Resource.GetBuildingSprite(buildingType, false, isActive, buttonInfo.dirType);


			if (!isActive) return;
			switch (buildingType) {
                case BuildingType.Button:
                    break;
                case BuildingType.ToggleButton:
					MapManager.Instance.ToggleButton(buttonInfo.linkedGridPos.x,
						buttonInfo.linkedGridPos.y);
					break;
                case BuildingType.RotateButton:
					MapManager.Instance.PadToOrigin(buttonInfo.linkedGridPos.x,
						buttonInfo.linkedGridPos.y);
					break;
            }
		}

        public void OnPressed()
        {
            if (isPressed) return;
            isPressed = true;
			buildingSprite.sprite = Managers.Resource.GetBuildingSprite(buildingType, true, isActive, buttonInfo.dirType);

            if (!isActive) return;
			switch (buildingType)
			{
				case BuildingType.Button:
					MapManager.Instance.AddPerson();
					break;
				case BuildingType.ToggleButton:
					MapManager.Instance.ToggleButton(buttonInfo.linkedGridPos.x,
						buttonInfo.linkedGridPos.y);
					break;
				case BuildingType.RotateButton:
					MapManager.Instance.RotatePadDir(buttonInfo.linkedGridPos.x,
						buttonInfo.linkedGridPos.y,
						buttonInfo.dirType);
					break;
			}
        }

        public void SetVisibility(InputMode mode)
        {

            switch (mode) { 
                case InputMode.None:
                    if(padType != PadType.DirNone) arrowSprite.color = Constants.COLOR_ARROW;
                    buildingSprite.color = Constants.COLOR_WHITE;
                    break;
                case InputMode.Pad:
                    if (padType != PadType.DirNone) arrowSprite.color = Constants.COLOR_ARROW;
                    buildingSprite.color = Constants.COLOR_INVISIBLE;
                    break;
                case InputMode.Building:
                    if (padType != PadType.DirNone) arrowSprite.color = Constants.COLOR_INVISIBLE;
                    buildingSprite.color = Constants.COLOR_WHITE;
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
            buttonInfo = new ButtonInfos(new Vector2Int(posX, posY));
		}

		public void ToggleActive()
        {
            if (buildingType == BuildingType.None) return;

            isActive = !isActive;
            buildingSprite.sprite = Managers.Resource.GetBuildingSprite(buildingType, isPressed, isActive, buttonInfo.dirType);
        }

        public void OnButtonRotate()
        {
            if (buttonInfo.dirType == PadType.DirNone)
            {
                buttonInfo.dirType = PadType.DirUp;
            }
            else
                buttonInfo.dirType = (PadType)((int)buttonInfo.dirType + 1);

            buildingSprite.sprite = Managers.Resource.GetBuildingSprite(BuildingType, isPressed, isActive, buttonInfo.dirType);
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

        private GunnersManagement gunnersManagement;
        private void InitGunners()
        {
            gunnersManagement = GetComponent<GunnersManagement>();
            gunnersManagement.PlaceGunners(mapSize);
        }
 
        private void Awake()
        {
            Init();
            InitGunners();
        }
        private StageInfo currentStageInfo;
        public StageInfo CurrentStageInfo { get => currentStageInfo; }
        private ChapterInfo currentChapterInfo;
        public ChapterInfo CurrentChapterInfo { get => currentChapterInfo; }
        private MapGrid[,] programMap;
        public MapGrid[,] ProgramMap { get => programMap; }

        [SerializeField] private Vector2Int mapSize;
        public Vector2Int MapSize { get => mapSize; }
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private GameObject humanPrefab;
        [SerializeField] private GameObject gunnerPrefab;

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
        }

        private bool isCycleRunning = false;
        private bool isCircuiting = false;
        private Vector2Int circuitingButtonPos;
        public Vector2Int prevHoverPos = new Vector2Int(0, 0);
        private bool isOneCycling = false; //1사이클씩 실행되고있는지
        public bool IsCircuiting { get => isCircuiting; }
        public bool IsOneCycling { get => isOneCycling; set => isOneCycling = value; }

		#region CycleLock
		private int cycleLock = 1;
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
			// HACK : 저장 타이밍 따로 정해줘야됨
			if (Input.GetKeyDown(KeyCode.S))
			{
				SaveStage();
			}

			if (!isCycleRunning)
            {
                if (!IsCycleEnabled()) return;
                StartCoroutine(ProgramCycleCoroutine());
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
        public BuildingType OnHoverMapGridInNoneMode(int x, int y)
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

                return BuildingType.None;
            }

            if (!CheckBoundary(x, y) || programMap[x, y].BuildingType == BuildingType.None)
            {
                buttonRect.gameObject.SetActive(false);
                tileRect.gameObject.SetActive(false);
                return BuildingType.None;
            }
            else
            {
                buttonRect.transform.position = new Vector3(x, y, Constants.HUMAN_POS_Z);
                buttonRect.sprite = programMap[x, y].BuildingSprite;
                buttonRect.gameObject.SetActive(true);
                
                if (programMap[x, y].BuildingType == BuildingType.Jump
				|| programMap[x, y].BuildingType == BuildingType.RotateButton
				|| programMap[x, y].BuildingType == BuildingType.ToggleButton)
				{
					tileRect.transform.position = new Vector3(programMap[x, y].ButtonInfo.linkedGridPos.x,
						programMap[x, y].ButtonInfo.linkedGridPos.y,
						Constants.HUMAN_POS_Z);
					tileRect.gameObject.SetActive(true);
				}
                else
                {
                    tileRect.gameObject.SetActive(false);
                }
                return programMap[x, y].BuildingType;
			}
        }

        public void OnClickMapGridInNoneMode(int x, int y, bool isSet)
        {
            if (!CheckBoundary(x, y))
            {
                isCircuiting = false;
                return;
            }

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
                if (programMap[x, y].BuildingType != BuildingType.Jump
                    && programMap[x, y].BuildingType != BuildingType.ToggleButton
                    && programMap[x, y].BuildingType != BuildingType.RotateButton) return;

                isCircuiting = true;
                circuitingButtonPos = new Vector2Int(x, y);
            }

        }

		public void OnRightClickMapGridInNoneMode(int x, int y)
        {
            if (!CheckBoundary(x, y)) return;

            if (programMap[x, y].BuildingType == BuildingType.ToggleButton) return;

            switch(programMap[x, y].BuildingType)
            {
                case BuildingType.RotateButton:
                    programMap[x, y].OnButtonRotate();
                    break;
                case BuildingType.Add1:
                case BuildingType.Sub1:
                case BuildingType.Jump:
                case BuildingType.Double:
                case BuildingType.Button:
					programMap[x, y].ToggleActive();
					break;
				case BuildingType.ToggleButton:
					break;
			}
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
				if (CheckBoundary(prevHoverPos.x, prevHoverPos.y))
					programMap[prevHoverPos.x, prevHoverPos.y].UnpreviewBuilding();
                prevHoverPos.Set(x, y);
                return;
            }
            if (prevHoverPos.x == x && prevHoverPos.y == y) return;

            if (CheckBoundary(prevHoverPos.x, prevHoverPos.y))
                programMap[prevHoverPos.x, prevHoverPos.y].UnpreviewBuilding();
			if (programMap[x, y].BuildingType == BuildingType.None)
				programMap[x, y].PreviewBuilding(type);
            prevHoverPos.Set(x, y);

        }
        public void OnClickMapGridInBuildingMode(int x, int y, BuildingType type)
        {
            if (!CheckBoundary(x, y)) return;
            programMap[x, y].SetBuilding(type);
            Managers.Input.OnInputModeChanged(InputMode.None);
        }
        public void OnRightClickMapGridInBuildingMode(int x, int y)
		{
			if (!CheckBoundary(x, y)) return;
            programMap[x, y].SetBuilding(BuildingType.None);
            programMap[x, y].ButtonInfo = new ButtonInfos(new Vector2Int(x, y));
		}


		public void OnInputModeChanged(InputMode mode)
		{
			if (CheckBoundary(prevHoverPos.x, prevHoverPos.y))
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
        public float CycleTime { get => cycleTime; }

        [SerializeField]private List<HumanController> humanControllers = new List<HumanController>();

        private List<bool> flags = new List<bool> { false, false, false };
        private List<float> timeSections = new List<float> { 0.3f, 0.5f, 0.7f };
        private List<Func<bool>> secFuncs = new List<Func<bool>>();
        private float cycleElapsedTime = 0f;

        /** Result Variables **/
		int cycleCount = 0;
		int killCount = 0;

		private IEnumerator ProgramCycleCoroutine()
        {
            InitPerCycle();
            cycleCount++;
            while (cycleElapsedTime < cycleTime)
            {
                ExecutePerFrame(cycleElapsedTime, cycleTime);
                yield return null;
                cycleElapsedTime += Time.deltaTime;
            }

            FinPerCycle();
        }


        private bool isPersonAdd = false;
        public bool IsPersonAdd { get => isPersonAdd; }


        public void DoubleCycleTime()
        {
            float prev = cycleTime;
            cycleTime = 0.5f;

            cycleElapsedTime = cycleElapsedTime * (cycleTime / prev);


        }
        public void AddPersonWith1x()
        {
            cycleCount = killCount = 0;
			float prev = cycleTime;
			cycleTime = 1f;
			cycleElapsedTime = cycleElapsedTime * (cycleTime / prev);

            if (idxIn == 0)
                isPersonAdd = true;
        }
        public void AddPersonWithOneCycling()
        {

            float prev = cycleTime;
            cycleTime = 0.1f;
            cycleElapsedTime = cycleElapsedTime * (cycleTime / prev);

            if (idxIn == 0)
                isPersonAdd = true;
        }
        public void AddPerson()
        {
			isPersonAdd = true;
		}

        /// <summary>
        /// 싸이클 시작할 떄 수행해야하는 애들
        /// 변수 값 초기화가 주 목적
        /// </summary>
        private int idxIn = 0; 
        public int IdxIn { get => idxIn; set => idxIn = value; }
        private void InitPerCycle()
        {
            isCycleRunning = true;
            InitNewPerson();
            InitTeleport();

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
            foreach(HumanController controller in humanControllers)
            {
                if (controller.TargetPos == controller.PrevPos) continue;
                if (!CheckBoundary(controller.CurrentPos.x, controller.CurrentPos.y)) continue;
                programMap[controller.CurrentPos.x, controller.CurrentPos.y].OnRelease();
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
            return true;
        }


        /// <summary>
        /// 2/3 경과 시, 이동한 위치의 Button을 Press 하고 변경사항 업데이트
        /// </summary>
        private bool ExecuteAtTwoThirds()
        {

            foreach (var controller in humanControllers)
            {
                if (!CheckBoundary(controller.TargetPos.x, controller.TargetPos.y)) continue;
                programMap[controller.TargetPos.x, controller.TargetPos.y].OnPressed();
            }
            return true;
        }

        private int idxOut = 0;
        public int IdxOut { get => idxOut; set => idxOut = value; }
        private bool isOutputCorrect = true;
        /// <summary>
        /// 전부 경과하여 이동 완료
        /// 1/2 에서 controller에 체크한 걸로 더하기 연산 먼저 수행
        /// 다음 이동할 위치 설정, 연산 수행(+1, +/- ...)
        /// </summary>
        private void FinPerCycle()
        {

            DoTeleport();

            CalcDuplicatedPos();

			foreach (var controller in humanControllers)
            {
                controller.ExecuteOperand();
            }
            humanControllers.RemoveAll(item => item.OperandType == HumanOperandType.Operand2);

			DoButtonExecution();

			for (int i = humanControllers.Count - 1; i >= 0; i--)
            {
                humanControllers[i].OnFinPerCycle();

                if (!(humanControllers[i].CurrentPos.x == mapSize.x - 1 && humanControllers[i].CurrentPos.y == mapSize.y))
                {
                    if (!CheckBoundary(humanControllers[i].CurrentPos.x, humanControllers[i].CurrentPos.y))
                    {
                        gunnersManagement.DetectEscaped(humanControllers[i].CurrentPos);
                        humanControllers[i].HumanDyingProcess();
                        humanControllers.Remove(humanControllers[i]);
                        killCount++;
                        continue;
                    }
                    continue;
                }
                //human이 output지점 (4,5)이 아닌 바운더리 안이면 continue, (4,5)를 제외한 바운더리 바깥이면 총쏴서 없앰

                if (idxOut < currentStageInfo.outputs.Length)
                {
                    if (humanControllers[i].HumanNum != currentStageInfo.outputs[idxOut])
                    {
                        isOutputCorrect = false;
                    }

                    // HACK - 이거 수정해야됨
                    // 아마 conflict 날 듯?
					GameManagerEx.Instance.Cameras[(int)GameManagerEx.Instance.CurrentCamType]
						.GetComponent<CameraBase>().CctvUI?.InOut.SetValue(IdxOut, false, humanControllers[i].HumanNum);
					idxOut++;
                }


				Destroy(humanControllers[i].gameObject);
                humanControllers.Remove(humanControllers[i]);
            }

            CheckIsStageEnded();

            isCycleRunning = false;
            
            if (isOneCycling)
                GameManagerEx.Instance.SetExeType(ExecuteType.Pause); // 게임 정지
        }


        private bool isStageEnded = true;
        // TODO - 이거 false되는 경우를 stop 누를 때랑 스테이지 처음 들어갈 때로 해야 할 듯, 지금 InitNewPerson에 있음
        private void CheckIsStageEnded() // 스테이지 끝났는지 여부 및 정답 체크하는 함수
        {
            if (GameManagerEx.Instance.ExeType == ExecuteType.None ||
                humanControllers.Count != 0) return;

            if (isStageEnded) return;

            isStageEnded = true;

            if (idxOut == currentStageInfo.outputs.Length && isOutputCorrect)
            {
                OnSuccess();
                // 팝업창 띄우면서 InputLock 필요
            }
            else
            {
                OnFailure();
            }
        }

        private void OnSuccess()
        {
            int btnCount = 0;

            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    btnCount += (programMap[i, j].BuildingType == BuildingType.None ? 0 : 1);
                }
            }

            GameResultInfo info = new GameResultInfo(currentChapter, currentStage, currentSaveIdx,
                cycleCount, btnCount, killCount);
            Debug.Log("Chapter, Stage : " + info.ChapterIdx.ToString() + ", " + info.StageIdx.ToString());
            GameManagerEx.Instance.OnStageSuccess(info);

            idxOut = 0;
            isOutputCorrect = true;
        }

        private void OnFailure()
        {
            Debug.Log("FAILURE");
            idxOut = 0;
            isOutputCorrect = true;
        }

        private void DoButtonExecution()
        {
            int size = humanControllers.Count;
            // Button 연산
            for (int i = 0; i < size; i++)
			{
				Vector2Int tmpV = humanControllers[i].CurrentPos;
                if (!CheckBoundary(tmpV.x, tmpV.y)) continue;
				if (programMap[tmpV.x, tmpV.y].BuildingType != BuildingType.None && programMap[tmpV.x, tmpV.y].IsPressed)
				{
                    if (!programMap[tmpV.x, tmpV.y].IsActive) continue;
                    switch(programMap[tmpV.x, tmpV.y].BuildingType)
                    {
                        case BuildingType.Add1:
							humanControllers[i].AddByButton();
							break;
                        case BuildingType.Sub1:
							humanControllers[i].SubByButton();
							break;
                        case BuildingType.Double:

							if (programMap[tmpV.x, tmpV.y].PadType == PadType.DirNone ||
                                (programMap[tmpV.x, tmpV.y].PadType == programMap[humanControllers[i].PrevPos.x, humanControllers[i].PrevPos.y].PadType))
                            {  // 경로가 겹치는 경우 바로 두배
                                humanControllers[i].HumanNum *= 2;
                            }
                            else
							{
								HumanController tmpController = Instantiate(humanPrefab, new Vector3(tmpV.x, tmpV.y, Constants.HUMAN_POS_Z), Quaternion.identity)
									.GetComponent<HumanController>();
								tmpController.SetAsDoubled(humanControllers[i]);
								humanControllers.Add(tmpController);
							}
							break;
					}
				}
			}
		}

        #endregion

        public void RotatePadDir(int x, int y, PadType type)
        {
            programMap[x, y].SetPad(type, false);
		}
		public void PadToOrigin(int x, int y)
		{
			programMap[x, y].SetPadToOrigin();
		}

        public void ToggleButton(int x, int y)
        {
            programMap[x, y].ToggleActive();
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


		private void InitNewPerson()
		{
			if (isPersonAdd && idxIn < currentStageInfo.inputs.Length)
			{
				HumanController tmpController = Instantiate(humanPrefab, new Vector3(0f, -1f, Constants.HUMAN_POS_Z), Quaternion.identity)
					.GetComponent<HumanController>();
				humanControllers.Add(tmpController);

				tmpController.HumanNum = currentStageInfo.inputs[idxIn];
				GameManagerEx.Instance.Cameras[(int)GameManagerEx.Instance.CurrentCamType]
	                .GetComponent<CameraBase>().CctvUI?.InOut.SetValue(idxIn, true);
				idxIn++;
				isPersonAdd = false;
                isStageEnded = false;
			}
		}

        private void InitTeleport()
        {
			foreach (HumanController controller in humanControllers)
            {
                if (!CheckBoundary(controller.CurrentPos.x, controller.CurrentPos.y)) return;
                if (programMap[controller.CurrentPos.x, controller.CurrentPos.y].BuildingType == BuildingType.Jump
                    && programMap[controller.CurrentPos.x, controller.CurrentPos.y].ButtonInfo.linkedGridPos.x >= 0)
                {
					controller.OnTeleport();
				}
			}
        }

        // 더해질 애들 계산
        private void CalcDuplicatedPos()
        {
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

		}

        private void DoTeleport()
		{
			foreach (var controller in humanControllers)
			{
                if (!controller.IsTeleport) continue;
				programMap[controller.CurrentPos.x, controller.CurrentPos.y].OnRelease();
				controller.OffTeleport(programMap[controller.CurrentPos.x, controller.CurrentPos.y].ButtonInfo.linkedGridPos);
                programMap[controller.CurrentPos.x, controller.CurrentPos.y].OnPressed();
			}
		}


		#region Save/Load Data

		private int currentStage = -1;

        public int CurrentStage { get => currentStage; }

        private int currentSaveIdx = -1;


        public void LoadStage(int stageId, int saveIdx)
        {
            currentStage = stageId;
            currentSaveIdx = saveIdx;
			currentStageInfo = Managers.Resource.GetStageInfo(stageId);

			// TODO : StageInfo에 따라 Map Width라던지 전부 setting 해야됨

			for (int i = 0; i < mapSize.x; i++)
			{
				for (int j = 0; j < mapSize.y; j++)
				{
                    programMap[i, j].ClearGrid();
				}
			}

			StageSaveData tmpGridData = Managers.Data.GetGridDatas(stageId, saveIdx);
            if (tmpGridData == null) return;

            foreach (var data in tmpGridData.gridDatas)
            {
                programMap[data.posX, data.posY].SetStageGridInfo(data);
            }

        }

        private int currentChapter = -1;

        public int CurrentChapter { get => currentChapter; }

        public void LoadChapter(int chapterId)
        {
            Debug.Log($"Load Chapter : {chapterId}");

            currentChapter = chapterId;
            currentChapterInfo = Managers.Resource.GetChapterInfo(chapterId);
        }

        public void SaveStage()
		{
            Debug.Log("SAVED");
			Debug.Log($"Save Stage : {currentStage}");

			StageSaveData gridDatas = new StageSaveData();

            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    gridDatas.gridDatas.Add(programMap[i, j].GetStageGridData());
                }
            }

            Managers.Data.AddStageGridData(currentStage, currentSaveIdx, gridDatas);
        }

        #endregion

        public void ClearHumans()
        {
            isPersonAdd = false;
            for (int i = humanControllers.Count - 1; i >= 0; i--)
            {
                humanControllers[i].HumanDyingProcess();
                humanControllers.Remove(humanControllers[i]);
            }

            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    programMap[i, j].OnRelease();
                }
            }
            cycleCount = killCount = 0;

		}

	}
}