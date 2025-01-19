using System.Linq;
using UnityEngine;

namespace HumanFactory.Manager
{
    /*
     * 게임의 리소스를 관리하는 매니저입니다.
     * 동적으로 리소스를 로드하는 경우,
     * 게임 시작할때 필요한 정보들을 저장해야하는 경우 등 사용합니다.
     * 
     * Load의 베이스 위치는 "Assets/Resources/" 입니다.
     * 따라서 Save는 하지 않습니다.
     */
    public class ResourceManager
    {
        /** Json file Paths**/
        private string stageInfoPath = "JsonData/StageData";

        /** Audio source Paths **/
        private string bgmPath = "Sounds/BGM";

        private string buildingPressedOnPath = "Sprites/Buildings/Pressed_On";
		private string buildingPressedOffPath = "Sprites/Buildings/Pressed_Off";
		private string buildingReleasedOnPath = "Sprites/Buildings/Released_On";
        private string buildingReleasedOffPath = "Sprites/Buildings/Released_Off";
        private string effectSpritePath = "Sprites/Effects";

        private string pressedRButtonPath = "Sprites/Buildings/Pressed_On/6_RotateButton";
        private string releasedRButtonPath = "Sprites/Buildings/Released_On/6_RotateButton";

        /** Data Containers **/
        private StageInfos stageInfos = new StageInfos();
        private ChapterInfos chapterInfos = new ChapterInfos();
        private AudioClip[] audioSources;
        private Sprite[] buildingPressedOnSprites;
        private Sprite[] buildingPressedOffSprites;
        private Sprite[] buildingReleasedOnSprites;
        private Sprite[] buildingReleasedOffSprites;

        private Sprite[] pressedRotateButtonSprites;
        private Sprite[] releasedRotateButtonSprites;

        private Sprite[] effectSprites;

        public void Init()
        {
            chapterInfos = JsonUtility.FromJson<ChapterInfos>(Resources.Load<TextAsset>(stageInfoPath).text);
            stageInfos = JsonUtility.FromJson<StageInfos>(Resources.Load<TextAsset>(stageInfoPath).text);
            audioSources = Resources.LoadAll<AudioClip>(bgmPath);

			buildingPressedOnSprites = Resources.LoadAll<Sprite>(buildingPressedOnPath);
			buildingPressedOffSprites = Resources.LoadAll<Sprite>(buildingPressedOffPath);
			buildingReleasedOnSprites = Resources.LoadAll<Sprite>(buildingReleasedOnPath);
			buildingReleasedOffSprites = Resources.LoadAll<Sprite>(buildingReleasedOffPath);

			pressedRotateButtonSprites = Resources.LoadAll<Sprite>(pressedRButtonPath);
			releasedRotateButtonSprites = Resources.LoadAll<Sprite>(releasedRButtonPath);

            effectSprites = Resources.LoadAll<Sprite>(effectSpritePath);
        }

        /** Getter Functions **/
        public StageInfo GetStageInfo(int id)
        {
            var ret = from info in stageInfos.stageInfo
                      where info.stageId == id
                      select info;

            if (ret.Count() != 1)
            {
                Debug.LogWarning("Stage Info Error : Check Ids in json file");
            }

            return ret.First();
        }

        public ChapterInfo GetChapterInfo(int id)
        {
            var ret = from info in chapterInfos.chapterInfo
                      where info.chapterId == id
                      select info;

            if (ret.Count() != 1)
            {
                Debug.LogWarning("Chapter Info Error : Check Ids in json file");
            }

            return ret.First();
        }

        public int GetStageCount()
        {
            return stageInfos.stageInfo.Length;
        }

        public int GetChapterCount()
        {
            return chapterInfos.chapterInfo.Length;
        }

        public AudioClip GetBGM(BGMType type)
        {
            if (audioSources.Length <= (int)type)
            {
                Debug.Log("Resource Error : There is not enough bgms..");
                return null;
            }

            return audioSources[(int)type];
        }

        public Sprite GetBuildingSprite(BuildingType type, bool isPressed, bool isActive
            , PadType padType = PadType.DirNone)
        {
            if (type == BuildingType.None) return null;

            if (type == BuildingType.RotateButton && padType != PadType.DirNone)
            {
                return (isPressed ? pressedRotateButtonSprites[(int)padType]
                    : releasedRotateButtonSprites[(int)padType]);
            }

            if (isActive)
				return (isPressed ? buildingPressedOnSprites[(int)type] :
					buildingReleasedOnSprites[(int)type]);
            else
                return (isPressed ? buildingPressedOffSprites[(int)type] :
					buildingReleasedOffSprites[(int)type]);
        }

        public Sprite GetEffectSprite(EffectType type)
        {
            return effectSprites[(int)type];
        }

    }
}