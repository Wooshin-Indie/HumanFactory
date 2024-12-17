using HumanFactory.Manager;
using System;
using UnityEngine;

namespace HumanFactory
{

    #region MapInfos

    public class ButtonInfos
    {
        public Vector2Int linkedGridPos;
        public ButtonType buttonType;
        public PadType dirType;

        public ButtonInfos(Vector2Int linkedGrid,
            ButtonType buttonType = ButtonType.NewInput, 
            PadType dirType = PadType.DirNone)
        {
            this.linkedGridPos = linkedGrid;
            this.buttonType = buttonType;
            this.dirType = dirType;
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
    public class StageInfo
    {
        public int stageId;
        public string stageName;
        // TODO : 스테이지에 따라 다른 데이터를 추가해야합니다.
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

        public float SfxVolume { get=> sfxVolume; set=> sfxVolume = value; }
        public float BgmVolume { get=> bgmVolume; set=> bgmVolume = value; }
        public bool IsRevealBlood { get=> isRevealBlood; set=> isRevealBlood = value; }
        public int LanguageIndex { get=> languageIndex; set=> languageIndex = value; }
        // etc...
    }

    [Serializable]
    public class GameplayData {     // 유저가 플레이한 게임 데이터를 저장. Start에서 
        //List<bool> stageCleared = new List<bool>();
    }
    #endregion
}