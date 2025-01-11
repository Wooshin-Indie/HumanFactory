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

        private string buildingPressedPath = "Sprites/Buildings/Pressed";
        private string buildingReleasedPath = "Sprites/Buildings/Released";
        private string effectSpritePath = "Sprites/Effects";

        /** Data Containers **/
        private StageInfos stageInfos = new StageInfos();
        private AudioClip[] audioSources;
        private Sprite[] buildingPressedSprites;
        private Sprite[] buildingReleasedSprites;
        private Sprite[] effectSprites;

        public void Init()
        {
            stageInfos = JsonUtility.FromJson<StageInfos>(Resources.Load<TextAsset>(stageInfoPath).text);
            audioSources = Resources.LoadAll<AudioClip>(bgmPath);

            buildingPressedSprites = Resources.LoadAll<Sprite>(buildingPressedPath);
            buildingReleasedSprites = Resources.LoadAll<Sprite>(buildingReleasedPath);
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

        public int GetStageCount()
        {
            return stageInfos.stageInfo.Length;
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

        public Sprite GetBuildingSprite(BuildingType type, bool isPressed)
        {
            if (type == BuildingType.None) return null;
            return (isPressed ? buildingPressedSprites[(int)type] :
                buildingReleasedSprites[(int)type]);
        }

        public Sprite GetEffectSprite(EffectType type)
        {
            return effectSprites[(int)type];
        }

    }
}