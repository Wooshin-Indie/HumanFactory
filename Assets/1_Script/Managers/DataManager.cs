using UnityEngine;
using System.IO;

namespace HumanFactory.Managers
{
    /*
     * persistentDataPath : C:/Users/[UserName]/AppData/LocalLow/DefaultCompany/[ProjectName]
     * 
     * 1. DataManager는 데이터를 저장/로드하기 위한 매니저입니다.
     * 2. 이 경로는 런타임에 읽고 쓰기 가능한 경로입니다. (해당 경로 없으면 로그 찍어보세요)
     * 3. 해당 데이터들은 git이 추적하지 않으므로 각자 관리하셔야 합니다. (오류나면 경로안에 있는 데이터 지우시면 됩니다)
     * 4. 위에 추가적인 정보저장을 위해 스크립트를 만든다면, 꼭 디폴트 생성자에서 변수 초기화해주세요.
     */
    public class DataManager
    {

        /** Paths **/
        private string settingDataPath;
        private string playDataPath;

        /** Datas **/
        private SettingData settingData = null;
        private GameplayData gameplayData = null;

        public void Init()
        {
            settingDataPath = Application.persistentDataPath + "/SettingData.json";
            playDataPath = Application.persistentDataPath + "/PlayData.json";

            LoadAll();
        }

        /** Save/Load Functions **/
        private void LoadAll()
        {
            LoadData<SettingData>(ref settingData, settingDataPath);
            LoadData<GameplayData>(ref gameplayData, playDataPath);
        }
        public void SaveAll()
        {
            SaveData<SettingData>(ref settingData, settingDataPath);
            SaveData<GameplayData>(ref gameplayData, playDataPath);
        }

        private void SaveData<T>(ref T data, string path)
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }

        private void LoadData<T>(ref T data, string path) where T : new()
        {
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                data = JsonUtility.FromJson<T>(json);
            }
            else
            {
                data = new T();

                Debug.Log(data.ToString());
            }
        }


        public void SaveClearStage(int stageId)
        {
            Debug.Log($"Stage: {stageId} Clear!");
            // TODO : GameplayData 에 진행상황 bool 배열로 저장해둬야합니다.
        }

    }


}