using HumanFactory.Manager;
using System;

namespace HumanFactory
{

    #region MapInfos

    public class ButtonInfos
    {
        public MapGrid linkedGrid;
        public ButtonType buttonType;
        public PadType dirType;

        public ButtonInfos(MapGrid linkedGrid,
            ButtonType buttonType = ButtonType.NewInput, 
            PadType dirType = PadType.DirUp)
        {
            this.linkedGrid = linkedGrid;
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

        // TODO : 스테이지에 따라 다른 데이터를 추가해야합니다.
    }
    #endregion

    #region DataManager Containers
    [Serializable]
    public class SettingData
    {     // Setting 저장 (소리 크기, 해상도..)

        public SettingData()
        {
            sfxVolume = 1f;
            bgmVolume = 1f;
        }

        public float sfxVolume;
        public float bgmVolume;

        // etc...
    }

    [Serializable]
    public class GameplayData {     // 유저가 플레이한 게임 데이터를 저장. Start에서 
        //List<bool> stageCleared = new List<bool>();
    }
    #endregion
}