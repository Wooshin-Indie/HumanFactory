using HumanFactory.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HumanFactory
{

    #region MapInfos

    [System.Serializable]
    public class ButtonInfos
    {
        public Vector2Int linkedGridPos;
        public PadType dirType;

        public ButtonInfos(Vector2Int linkedGrid,
            PadType dirType = PadType.DirNone)
        {
            this.linkedGridPos = linkedGrid;
            this.dirType = dirType;
        }

        public ButtonInfos(ButtonInfos info)
        {
			this.linkedGridPos = info.linkedGridPos;
			this.dirType = info.dirType;
		}

        public void ChangePadType(bool isNext)
        {
            if (isNext)
            {
                dirType = (PadType)(((int)dirType + 1) % Enum.GetNames(typeof(PadType)).Length);
            }
            else
            {
                dirType--;
                if ((int)dirType == -1)
                {
                    dirType = PadType.DirNone;
                }
            }
        }
    }

    public class GameResultInfo
    {
        private int chapterIdx = 0;
        private int stageIdx = 0;
        private int cctvIdx = 0;
        StageResultData stageResultData;

        public int ChapterIdx { get { return chapterIdx; } }
        public int StageIdx { get { return stageIdx; } }
        public int CctvIdx { get { return cctvIdx; } }
        public int CycleCount { get { return stageResultData.cycleCount; } }
        public int ButtonCount { get { return stageResultData.buttonCount; } }
        public int KillCount { get { return stageResultData.killCount; } }
        public StageResultData ResultData { get => stageResultData; }
        public GameResultInfo(int chapter, int stage, int cctv, int cycle, int button, int kill)
        {
            SetCounts(chapter, stage, cctv, cycle, button, kill);
        }

        public void SetCounts(int chapter, int stage, int cctv, int cycle, int button, int kill)
        {
            Debug.Log($"Chap : {chapter}, {stage}, {cctv}, Cycle: {cycle}, BtnCnt: {button}, Kill: {kill}");
            chapterIdx = chapter;
            stageIdx = stage;
            cctvIdx = cctv;
            stageResultData = new StageResultData(cycle, button, kill);
        }
    }

    [Serializable]
    public class StageResultData
    {
        public int cycleCount = -1;
		public int buttonCount = -1;
		public int killCount = -1;

        public StageResultData(int cycle, int button, int kill)
        {
            cycleCount = cycle;
            buttonCount = button;
            killCount = kill;
        }

        public void UpdateData(StageResultData data)
        {
            cycleCount = (cycleCount < 0) ? data.cycleCount : Mathf.Min(data.cycleCount, cycleCount);
			buttonCount = (buttonCount < 0) ? data.buttonCount : Mathf.Min(data.buttonCount, buttonCount);
			killCount = (killCount < 0) ? data.killCount : Mathf.Min(data.killCount, killCount);
        }

		public override string ToString()
		{
            return $"CYCLE : {cycleCount}, BUTTON: {buttonCount}, KILL :{killCount}";
		}
	}


    #endregion

    #region ResourceManager Containers
    [Serializable]
    public class StageInfos
    {
        public StageInfo[] stageInfo;
    }

    [Serializable]
    public class ChapterInfos
    {
        public ChapterInfo[] chapterInfo;
    }

    [Serializable]
    public class StageInfo
    {
        public int stageId;
        public int[] inputs;
        public int[] outputs;
        public int[] enableBuildings;
        public int[] challenges;
        public bool isExpanded;
        public int prerequisite;
        public int[] maxCounts;
	}

    [Serializable]
    public class  ChapterInfo
    {
        public int chapterId;
        public int[] stageIndexes;
    }
    #endregion

    #region DataManager Containers
    [Serializable]
    public class SettingData
    {
        public SettingData()
        {
            sfxVolume = 1f;
            bgmVolume = 1f;
            masterVolume = 1f;
            languageIndex = 0;
            isRevealBlood = true;
			resolutionWidth = 1920;
			resolutionHeight= 1080;
            isFullScreen = true;
			isScanline = true;
        }


        public float masterVolume;
        public float sfxVolume;
        public float bgmVolume;
        public bool isRevealBlood;
        public int languageIndex;
        public int[] keyBindings;
        public int resolutionWidth;
        public int resolutionHeight;
        public bool isFullScreen;
        public bool isScanline;

        // etc...

        public void Clear()
		{
			sfxVolume = 1f;
			bgmVolume = 1f;
			masterVolume = 1f;
			languageIndex = 0;
			isRevealBlood = true;
			resolutionWidth = 1920;
			resolutionHeight = 1080;
			isFullScreen = true;
			isScanline = true;

            keyBindings = new int[Constants.KEYCODE_SHORTCUT_DEFAULT.Length];
            for (int i = 0; i < Constants.KEYCODE_SHORTCUT_DEFAULT.Length; i++)
            {
                keyBindings[i] = (int)Constants.KEYCODE_SHORTCUT_DEFAULT[i];
            }
		}
    }

    [Serializable]
    public class StageGridData
    {
        public int posX, posY;
        public bool isActive;
        public PadType padtype;
        public BuildingType buildingType;
        public ButtonInfos buttonInfos;
    }

    [Serializable]
	public class StageSaveData
	{
		public List<StageGridData> gridDatas = new List<StageGridData>();
	}

	[Serializable]
    public class StageGridDatas
	{
        public StageGridDatas()
        {
            for(int i=0; i<saveDatas.Count; i++)
            {
                saveDatas[i] = new StageSaveData();
            }
        }
		public List<StageSaveData> saveDatas = new List<StageSaveData>(new StageSaveData[5]);
	}

    [Serializable]
    public class GameplayData 
    {
        public StageGridDatas[] stageGridDatas;
	}

    [Serializable]
    public class PlayerResultData
    {
        public StageResultData[] resultDatas;
	}

    #endregion

    #region Server Containers

    /// <summary>
    /// Client에서 서버에 보낼 시뮬레이션 데이터
    /// </summary>
    public class ClientSimulationData
    {
        public int userId = 0;
        public int stageIdx = 0;
        public StageSaveData saveData;
    }

    public class SimulationResult
    {
        public int userId = 0;
        public int stageIdx = 0;
        public int cycleCount = -1;
        public int buttonCount = -1;
        public int killCount = -1;

        public SimulationResult(int uid, GameResultInfo info)
        {
            userId = uid;
            stageIdx = info.StageIdx;
            cycleCount = info.CycleCount;
            buttonCount = info.ButtonCount;
            killCount = info.KillCount;
        }
    }

    /// <summary>
    /// 서버에서 클라이언트로 보낼 결과 데이터
    /// </summary>
    [System.Serializable]
    public class ServerResultData
    {
        public CountResultData[] datas;

        public ServerResultData()
        {
        }

        public void Set()
        {
			datas = new CountResultData[Managers.Resource.GetStageCount()];

			for (int i = 0; i < datas.Count(); i++)
			{
                datas[i] = new CountResultData();
				datas[i].cycleGraphs = Enumerable.Repeat(0, Constants.COUNT_GRAPH_MAX).ToArray();
				datas[i].buttonGraphs = Enumerable.Repeat(0, Constants.COUNT_GRAPH_MAX).ToArray();
				datas[i].killGraphs = Enumerable.Repeat(0, Constants.COUNT_GRAPH_MAX).ToArray();
			}
		}

        public void InsertData(int stageIdx, int cycleCnt, int btnCnt, int killCnt)
        {
            int cycleMax = Managers.Resource.GetStageInfo(stageIdx).maxCounts[0];
            int btnMax = Managers.Resource.GetStageInfo(stageIdx).maxCounts[1];
            int killMax = Managers.Resource.GetStageInfo(stageIdx).maxCounts[2];

            int cycleIdx = GetBarIdx(cycleCnt, cycleMax);
            int btnIdx = GetBarIdx(btnCnt, btnMax);
            int killIdx = GetBarIdx(killCnt, killMax);

            if (cycleIdx > 0) datas[stageIdx].cycleGraphs[cycleIdx]++;
            if (btnIdx > 0) datas[stageIdx].buttonGraphs[btnIdx]++;
            if (killIdx > 0) datas[stageIdx].killGraphs[killIdx]++;
		}

        private int GetBarIdx(int cnt, int max)
        {
            if (max == 0) return -1;
            return Mathf.Min((cnt * Constants.COUNT_GRAPH_MAX / max), Constants.COUNT_GRAPH_MAX - 1);
        }
    }

    [System.Serializable]
    public class CountResultData
	{
		public int[] cycleGraphs = new int[0];
		public int[] buttonGraphs = new int[0];
		public int[] killGraphs = new int[0];
    }
    
    #endregion
}