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

        private string pressedOnRButtonPath = "Sprites/Buildings/Pressed_On_Rotate";
        private string releasedOnRButtonPath = "Sprites/Buildings/Released_On_Rotate";
        private string pressedOffRButtonPath = "Sprites/Buildings/Pressed_Off_Rotate";
        private string releasedOffRButtonPath = "Sprites/Buildings/Released_Off_Rotate";

        private string pressedOnDButtonPath = "Sprites/Buildings/Pressed_On_Double";
        private string releasedOnDButtonPath = "Sprites/Buildings/Released_On_Double";
        private string pressedOffDButtonPath = "Sprites/Buildings/Pressed_Off_Double";
        private string releasedOffDButtonPath = "Sprites/Buildings/Released_Off_Double";

        /** Data Containers **/
        private StageInfos stageInfos = new StageInfos();
        private ChapterInfos chapterInfos = new ChapterInfos();
        private AudioClip[] audioSources;
        private Sprite[] buildingPressedOnSprites;
        private Sprite[] buildingPressedOffSprites;
        private Sprite[] buildingReleasedOnSprites;
        private Sprite[] buildingReleasedOffSprites;

        private Sprite[] pressedOnRotateButtonSprites;
        private Sprite[] releasedOnRotateButtonSprites;
        private Sprite[] pressedOffRotateButtonSprites;
        private Sprite[] releasedOffRotateButtonSprites;

        private Sprite[] pressedOnDoubleButtonSprites;
        private Sprite[] releasedOnDoubleButtonSprites;
        private Sprite[] pressedOffDoubleButtonSprites;
        private Sprite[] releasedOffDoubleButtonSprites;

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

			pressedOnRotateButtonSprites = Resources.LoadAll<Sprite>(pressedOnRButtonPath);
			releasedOnRotateButtonSprites = Resources.LoadAll<Sprite>(releasedOnRButtonPath);
			pressedOffRotateButtonSprites = Resources.LoadAll<Sprite>(pressedOffRButtonPath);
			releasedOffRotateButtonSprites = Resources.LoadAll<Sprite>(releasedOffRButtonPath);

            pressedOnDoubleButtonSprites = Resources.LoadAll<Sprite>(pressedOnDButtonPath);
            releasedOnDoubleButtonSprites = Resources.LoadAll<Sprite>(releasedOnDButtonPath);
            pressedOffDoubleButtonSprites = Resources.LoadAll<Sprite>(pressedOffDButtonPath);
            releasedOffDoubleButtonSprites = Resources.LoadAll<Sprite>(releasedOffDButtonPath);

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

            if (type == BuildingType.Rotate && padType != PadType.DirNone)
            {
                if (isActive)
                {
                    return (isPressed ? pressedOnRotateButtonSprites[(int)padType]
                        : releasedOnRotateButtonSprites[(int)padType]);
                }
                else
				{
					return (isPressed ? pressedOffRotateButtonSprites[(int)padType]
						: releasedOffRotateButtonSprites[(int)padType]);
				}
            }

            if (type == BuildingType.Double && padType != PadType.DirNone)
            {
                if (isActive)
                {
                    return (isPressed ? pressedOnDoubleButtonSprites[(int)padType]
                        : releasedOnDoubleButtonSprites[(int)padType]);
                }
                else
                {
                    return (isPressed ? pressedOffDoubleButtonSprites[(int)padType]
                        : releasedOffDoubleButtonSprites[(int)padType]);
                }
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