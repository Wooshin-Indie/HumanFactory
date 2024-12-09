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

        /** Data Containers **/
        private StageInfos stageInfos = new StageInfos();
        private AudioClip[] audioSources;

        public void Init()
        {
            stageInfos = JsonUtility.FromJson<StageInfos>(Resources.Load<TextAsset>(stageInfoPath).text);
            audioSources = Resources.LoadAll<AudioClip>(bgmPath);
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
                Debug.LogError("Resource Error : There is not enough bgms..");
                return null;
            }

            return audioSources[(int)type];
        }

    }
}