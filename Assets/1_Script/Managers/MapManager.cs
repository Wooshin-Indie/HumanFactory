using DG.Tweening;
using HumanFactory.Controller;
using HumanFactory.UI;
using HumanFactory.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HumanFactory.Manager
{
    public partial class MapManager : MonoBehaviour
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

		#region Map Expension
		private int currentMapIdx;
        public int CurrentMapIdx
        {
            get => currentMapIdx;
            set
            {
                OnCurrentMapIdxSet(value);
            }
        }

		private float[,] mapIdxPos = new float[4, 2]
        {
            {2f, 2f},
            {10f, 2f},
            {2f, 9f},
            {10f, 9f}
        };
        private Vector2Int mapInterval = new Vector2Int(3, 2);
        public Vector2Int MapInterval
        {
            get => mapInterval;
        }
        private bool isMapExpanded = false;
        public bool IsMapExpanded { get => isMapExpanded; }

		private bool isMapZoomed = true;
        private float zoomInOrthoSize = 5.5f;
        private float zoomOutOrthoSize = 8.5f;
        private Vector2 zoomOutPos = new Vector2(6f, 5.5f);

		public Action<int, bool> OnCurrentMapIdxAction { get; set; }

        // -1 이 zoom out 임
		private void OnCurrentMapIdxSet(int idx)
		{
			currentMapIdx = idx;
            OnCurrentMapIdxAction.Invoke(idx, isMapExpanded);
            if(idx < 0)
			{
				GameManagerEx.Instance.Cameras[(int)CameraType.Game].transform.DOLocalMove(new Vector3(zoomOutPos.x, zoomOutPos.y, Constants.CAMERA_POS_Z), 0.5f)
					.SetEase(Ease.OutQuart);
				GameManagerEx.Instance.Cameras[(int)CameraType.Game].DOOrthoSize(zoomOutOrthoSize, .5f).SetEase(Ease.OutQuart);
			}
            else
			{
				GameManagerEx.Instance.Cameras[(int)CameraType.Game].transform.DOLocalMove(new Vector3(mapIdxPos[idx, 0], mapIdxPos[idx, 1], Constants.CAMERA_POS_Z), 0.5f)
					.SetEase(Ease.OutQuart);
				GameManagerEx.Instance.Cameras[(int)CameraType.Game].DOOrthoSize(zoomInOrthoSize, .5f).SetEase(Ease.OutQuart);
			}
		}
		public void ToggleZoomMap()
		{
            if (!isMapExpanded) return;

            Managers.Sound.PlaySfx(SFXType.Zoom);
            if (isMapZoomed)
			{
				isMapZoomed = false;
				CurrentMapIdx = -1;
            }
            else
			{
				isMapZoomed = true;
				CurrentMapIdx = 0;
            }
		}
		#endregion


		private StageInfo currentStageInfo;
        public StageInfo CurrentStageInfo { get => currentStageInfo; }
        private ChapterInfo currentChapterInfo;
        public ChapterInfo CurrentChapterInfo { get => currentChapterInfo; }
        private MapGrid[,] programMap;
        public MapGrid[,] ProgramMap { get => programMap; }

        [SerializeField] private Vector2Int mapSize;
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private GameObject spritePrefab;
        [SerializeField] private GameObject humanPrefab;

        [Header("Circuit")]
        [SerializeField] private SpriteRenderer buttonRect;
        [SerializeField] private SpriteRenderer tileRect;

        private void Start()
        {
            buttonRect.gameObject.SetActive(false);
            tileRect.gameObject.SetActive(false);
            currentStageInfo = Managers.Resource.GetStageInfo(0);

			programMap = new MapGrid[mapSize.x * 2 +mapInterval.x, mapSize.y * 2 + mapInterval.y];
			InitTilemap();

			for (int i = 0; i < mapSize.x * 2 + mapInterval.x; i++)
            {
                for (int j = 0; j < mapSize.y * 2 + mapInterval.y; j++)
                {
                    programMap[i, j] = new MapGrid(i, j, 
                        Instantiate(arrowPrefab, new Vector3(i, j, 0f), Quaternion.identity).GetComponent<SpriteRenderer>(),
                        Instantiate(spritePrefab, new Vector3(i, j, 0f), Quaternion.identity).GetComponent<SpriteRenderer>()
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
		[SerializeField] private int cycleLock = 1;
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

			// HACK - Server 연결 테스트 입니다.
			if (Input.GetKeyDown(KeyCode.RightBracket))
			{
				Managers.Client.SendMessage();
			}

			if (!isCycleRunning)
            {
                if (!IsCycleEnabled())
                {
					foreach (HumanController controller in humanControllers)
					{
						controller.GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_IDLE);
					}
					return;
                }
                StartCoroutine(ProgramCycleCoroutine());
            }


		}

		public MapGrid GetMapGrid(int x, int y)
        {
            return programMap[x, y];
        }
        public bool CheckBoundary(int x, int y, bool isMapExpanded, int idx = -1)
        {
            if (!isMapExpanded)
			{
				return (x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y);
			}

			switch (idx) {
                case 0:
					return (x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y);
                case 1:
                    return (x >= mapSize.x + mapInterval.x && y >= 0 && x < mapSize.x * 2 + mapInterval.x && y < mapSize.y);
                case 2:
                    return (x >= 0 && y >= mapSize.y + mapInterval.y && x < mapSize.x && y < mapSize.y * 2 + mapInterval.y);
				case 3:
                    return (x >= mapSize.x + mapInterval.x && y >= mapSize.y + mapInterval.y && x < mapSize.x * 2 + mapInterval.x && y < mapSize.y * 2 + mapInterval.y);
                case -1:
                    return (x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y) ||
                        (x >= mapSize.x + mapInterval.x && y >= 0 && x < mapSize.x * 2 + mapInterval.x && y < mapSize.y) ||
                        (x >= 0 && y >= mapSize.y + mapInterval.y && x < mapSize.x && y < mapSize.y * 2 + mapInterval.y) ||
                        (x >= mapSize.x + mapInterval.x && y >= mapSize.y + mapInterval.y && x < mapSize.x * 2 + mapInterval.x && y < mapSize.y * 2 + mapInterval.y);
			}

            return false;
        }
        public int GetMapIdxFromPos(int x, int y, bool isMapExpanded)
        {
            if (x >= 0 && y >= 0 && x < mapSize.x && y < mapSize.y) return 0;
            if (!isMapExpanded) return -1;

            if (x >= mapSize.x + mapInterval.x && y >= 0 && x < mapSize.x * 2 + mapInterval.x && y < mapSize.y) return 1;
            if (x >= 0 && y >= mapSize.y + mapInterval.y && x < mapSize.x && y < mapSize.y * 2 + mapInterval.y) return 2;
            if (x >= mapSize.x + mapInterval.x && y >= mapSize.y + mapInterval.y && x < mapSize.x * 2 + mapInterval.x && y < mapSize.y * 2 + mapInterval.y) return 3;
            return -1;
        }

        [Header("Game Cycle")]
        [SerializeField, Range(0.5f, 2.0f)] private float cycleTime;
        [SerializeField] private List<HumanController> humanControllers = new List<HumanController>();

        private void OnSuccess()
        {
            Debug.Log("ON SUCCESS");
            int btnCount = 0;

            for (int i = 0; i < mapSize.x * 2 + mapInterval.x; i++)
            {
                for (int j = 0; j < mapSize.y * 2 + mapInterval.y; j++)
                {
                    btnCount += (programMap[i, j].BuildingType == BuildingType.None ? 0 : 1);
                }
            }

            GameResultInfo info = new GameResultInfo(currentChapter, currentStage, currentSaveIdx,
                cycleCount, btnCount, killCount);
            GameManagerEx.Instance.OnStageSuccess(info);
			GameManagerEx.Instance.SetExeType(ExecuteType.None);

            // Init Values

			ClearHumans();
			isOutputCorrect = true;
			isPersonAdd = false;
			isOneCycling = false;
            CycleTime = 1f;
        }
        private void OnFailure()
        {
            Debug.Log("FAILURE");

			GameResultInfo info = new GameResultInfo(currentChapter, currentStage, currentSaveIdx,
				cycleCount, -1, killCount);
			GameManagerEx.Instance.OnStageFail(info);
			GameManagerEx.Instance.SetExeType(ExecuteType.None);

			ClearHumans();
			isOutputCorrect = true;
			isPersonAdd = false;
			isOneCycling = false;
			CycleTime = 1f;
		}

        public void RotatePadDir(int x, int y, PadType type)
        {
            programMap[x, y].SetPad(type, false);
		}
		public void PadToOrigin(int x, int y)
		{
			programMap[x, y].SetPadToOrigin();
		}
        public void ToggleButtonInGame(int x, int y)
        {
            programMap[x, y].ToggleActive(true);
        }
		private void ChangeMapVisibility(InputMode mode)
        {
            for (int i = 0; i < mapSize.x * 2 + mapInterval.x; i++)
            {
                for (int j = 0; j < mapSize.y * 2  + mapInterval.y; j++)
                {
                    programMap[i, j].SetVisibility(mode);
                }
            }
        }

		#region Save/Load Data

		private int currentStage = -1;

        public int CurrentStage { get => currentStage; }

        private int currentSaveIdx = -1;


        public void LoadStage(int stageId, int saveIdx)
		{
			currentSaveIdx = saveIdx;
            LoadStage(stageId, Managers.Data.GetGridDatas(stageId, saveIdx));
        }

        public void LoadStage(int stageId, StageSaveData saveData)
		{
			/** Load Datas **/
			currentStage = stageId;
			currentStageInfo = Managers.Resource.GetStageInfo(stageId);

            /** Set Datas **/
            GameManagerEx.Instance.RayCasters[(int)CameraType.Game].GetComponent<BuildingPanelUI>()?.SetBanner();
			isMapExpanded = currentStageInfo.isExpanded;
			SetTilemap(isMapExpanded);
			gunnersManagement.LoadGunners(isMapExpanded);
			CurrentMapIdx = 0;

			Debug.Log("LOAD STAGE");

			for (int i = 0; i < mapSize.x * 2 + mapInterval.x; i++)
			{
				for (int j = 0; j < mapSize.y * 2 + mapInterval.y; j++)
				{
					programMap[i, j].ClearGrid();
				}
			}

			if (saveData == null) return;

			foreach (var data in saveData.gridDatas)
			{
				programMap[data.posX, data.posY].SetStageGridInfo(data);
			}

			ClearHumans();
		}

        [Header("Tilemap")]
        [SerializeField] private Tilemap gameTilemap;
        [SerializeField] private Tile inTile;
        [SerializeField] private Tile leftTile;
        [SerializeField] private Tile rightTile;
        [SerializeField] private Tile belowTile;
        [SerializeField] private Tile leftBelowTile;
        [SerializeField] private Tile rightBelowTile;
        [SerializeField] private int tilemapMargin;


        private void InitTilemap()
        {
            for(int i=-tilemapMargin;i < mapSize.x *2 + mapInterval.x + tilemapMargin; i++)
            {
                for(int j=-tilemapMargin;j < mapSize.y *2 + mapInterval.y + tilemapMargin; j++)
                {
                    gameTilemap.SetTile(new Vector3Int(i, j, 0), null);
                }
            }
        }

        private void SetTilemap(bool isExpanded)
		{

            int[,] mapOffsets = new int[4, 2]
            {
                {0, 0},
                {mapSize.x + mapInterval.x, 0},
                {0, mapSize.y + mapInterval.y},
                {mapSize.x + mapInterval.x, mapSize.y + mapInterval.y}
            };

            if (isExpanded)
            {
                for (int t = 0; t < mapOffsets.GetLength(0); t++)
				{
					for (int i = 0; i < mapSize.x; i++)
					{
						for (int j = 0; j < mapSize.y; j++)
						{
							gameTilemap.SetTile(new Vector3Int(mapOffsets[t, 0] + i, mapOffsets[t, 1] + j, 0), inTile);
						}
					}
                    for (int i = 0; i < mapSize.y; i++)
                    {
                        gameTilemap.SetTile(new Vector3Int(mapOffsets[t, 0] - 1, mapOffsets[t, 1] + i, 0), leftTile);
                        gameTilemap.SetTile(new Vector3Int(mapOffsets[t, 0] + mapSize.x, mapOffsets[t, 1] + i, 0), rightTile);
					}
					for (int i = 0; i < mapSize.x; i++)
					{
                        gameTilemap.SetTile(new Vector3Int(mapOffsets[t, 0] + i, mapOffsets[t, 1] - 1, 0), belowTile);
					}
                    gameTilemap.SetTile(new Vector3Int(mapOffsets[t, 0] - 1, mapOffsets[t, 1] - 1, 0), leftBelowTile);
                    gameTilemap.SetTile(new Vector3Int(mapOffsets[t, 0] + mapSize.x, mapOffsets[t, 1] - 1, 0), rightBelowTile);
				}
			}
            else
			{
				for (int t = 0; t < mapOffsets.GetLength(0); t++)
				{
					for (int i = 0; i <= mapSize.x; i++)
					{
						for (int j = 0; j < mapSize.y; j++)
						{
							gameTilemap.SetTile(new Vector3Int(mapOffsets[t, 0] + i, mapOffsets[t, 1] + j, 0),
								(t == 0) ? inTile : null);
						}

					}

					for (int i = 0; i < mapSize.y; i++)
					{
						gameTilemap.SetTile(new Vector3Int(mapOffsets[t, 0] - 1, mapOffsets[t, 1] + i, 0), (t == 0) ? leftTile : null);
						gameTilemap.SetTile(new Vector3Int(mapOffsets[t, 0] + mapSize.x, mapOffsets[t, 1] + i, 0), (t == 0) ? rightTile : null);
					}
					for (int i = 0; i < mapSize.x; i++)
					{
						gameTilemap.SetTile(new Vector3Int(mapOffsets[t, 0] + i, mapOffsets[t, 1] - 1, 0), (t == 0) ? belowTile : null);
					}
					gameTilemap.SetTile(new Vector3Int(mapOffsets[t, 0] - 1, mapOffsets[t, 1] - 1, 0), (t == 0) ? leftBelowTile : null);
					gameTilemap.SetTile(new Vector3Int(mapOffsets[t, 0] + mapSize.x, mapOffsets[t, 1] - 1, 0), (t == 0) ? rightBelowTile : null);
				}
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
			Debug.Log($"Save Stage : {currentStage}");
            if (currentStage < 0) return;

			StageSaveData gridDatas = new StageSaveData();

            for (int i = 0; i < mapSize.x * 2 + mapInterval.x; i++)
            {
                for (int j = 0; j < mapSize.y *2  + mapInterval.y; j++)
                {
                    if (programMap[i, j].PadType == PadType.DirNone &&
                        programMap[i, j].BuildingType == BuildingType.None) continue;
                    gridDatas.gridDatas.Add(programMap[i, j].GetStageGridData());
                }
            }

            Managers.Data.AddStageGridData(currentStage, currentSaveIdx, gridDatas);
            Managers.Data.SaveAll();
        }

        #endregion

        public void ClearHumans()
        {
            if (humanControllers.Count > 0)
                gunnersManagement.ClearHumans(isMapExpanded);

            isPersonAdd = false;
            for (int i = humanControllers.Count - 1; i >= 0; i--)
            {
                humanControllers[i].SetAnimSpeed(1f);
                humanControllers[i].HumanDyingProcessWithBox();
                humanControllers.Remove(humanControllers[i]);
            }

            for (int i = 0; i < mapSize.x * 2 + mapInterval.x; i++)
            {
                for (int j = 0; j < mapSize.y * 2 + mapInterval.y; j++)
                {
                    programMap[i, j].OnRelease();
                }
			}

			GameManagerEx.Instance.Cameras[(int)GameManagerEx.Instance.CurrentCamType]
				.GetComponent<CameraBase>().CctvUI?.InOut.OnClear();
            isOutputCorrect = true;
			cycleCount = 0;
			killCount = 0;
			idxIn = 0;
			idxOut = 0;
			CycleTime = 1f;
		}

        public float GetElapsedAnimTime()
        {
            for (int i = 0; i < programMap.GetLength(0); i++)
            {
                for (int j = 0; j < programMap.GetLength(1); j++)
                {
                    if (programMap[i, j].PadType != PadType.DirNone)
                    {
                        return programMap[i, j].GetPadAnimNormalizedTime();
					}
                }
            }

            return 0f;
        }

	}
}