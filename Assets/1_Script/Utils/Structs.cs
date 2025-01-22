using System;
using System.Collections.Generic;
using UnityEngine;

namespace HumanFactory
{

    #region MapInfos

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
        private int cycleCount = 0;
        private int buttonCount = 0;
        private int killCount = 0;

        public GameResultInfo(int chapter, int stage, int cctv, int cycle, int button, int kill)
        {
            SetCounts(chapter, stage, cctv, cycle, button, kill);
        }

        public void SetCounts(int chapter, int stage, int cctv, int cycle, int button, int kill)
        {
            Debug.Log($"Chap : {chapter}, {stage}, {cctv}, Cycle: {cycle}, BtnCnt: {button}, Kill: {kill}");
            cycleCount = cycle;
            buttonCount = button;
            killCount = kill;
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
        public string stageName;
        public int[] inputs;
        public int[] outputs;
        // TODO : 스테이지에 따라 다른 데이터를 추가해야합니다.
    }

    [Serializable]
    public class  ChapterInfo
    {
        public int chapterId;
        public string chapterName;
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
            languageIndex = 0;
            isRevealBlood = true;
        }

        private float sfxVolume;
        private float bgmVolume;
        private bool isRevealBlood;
        private int languageIndex;
        private int[] keyBindings;

        public float SfxVolume { get=> sfxVolume; set=> sfxVolume = value; }
        public float BgmVolume { get=> bgmVolume; set=> bgmVolume = value; }
        public bool IsRevealBlood { get=> isRevealBlood; set=> isRevealBlood = value; }
        public int LanguageIndex { get=> languageIndex; set=> languageIndex = value; }
        public int[] KeyBindings { get=> keyBindings; set=> keyBindings = value; }

        // etc...
    }

    [Serializable]
    public class StageGridData
    {
        public int posX, posY;
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
    #endregion
}