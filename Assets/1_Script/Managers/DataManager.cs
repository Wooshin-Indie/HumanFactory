using UnityEngine;
using System.IO;
using System;

namespace HumanFactory.Manager
{
    /*
     * persistentDataPath   : C:/Users/[UserName]/AppData/LocalLow/[DefaultCompany]/[ProjectName] (Windows)
     *                      : /home/[UserName]/.config/unity3d/[DefaultCompany]/[ProjectName]     (Ubuntu)
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
        private string serverResultPath;

        /** Datas **/
        private SettingData settingData = null;
        private GameplayData gameplayData = null;

        /** Properties (for outer uses) **/
        public SettingData BasicSettingData { get { return settingData; } }
        public GameplayData GamePlayData { get { return gameplayData; } }

        public Action OnUpdateKeyBindings { get; set; }
        public Action<SettingData> OnUpdateBasicSettings { get; set; }

		public void Init()
        {
            settingDataPath = Application.persistentDataPath + "/SettingData.json";
            playDataPath = Application.persistentDataPath + "/PlayData.json";
            serverResultPath = Application.persistentDataPath + "/ServerResultData.json";

            LoadAll();
            ApplyBasicSettings();
        }

        /** Save/Load Functions **/
        private void LoadAll()
        {
            LoadData<SettingData>(ref settingData, settingDataPath);
            LoadData<GameplayData>(ref gameplayData, playDataPath);

            InitSettingData();

            if (gameplayData.stageGridDatas == null ||
				gameplayData.stageGridDatas.Length != Managers.Resource.GetStageCount() + 1)
            {
                Array.Resize(ref gameplayData.stageGridDatas, Managers.Resource.GetStageCount() + 1);

                for (int i = 0; i < gameplayData.stageGridDatas.Length; i++){
                    if (gameplayData.stageGridDatas[i] == null)
                        gameplayData.stageGridDatas[i] = new StageGridDatas();
                }
            }
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
            }
        }

        public StageSaveData GetGridDatas(int stageId, int saveIdx)
        {
            return gameplayData.stageGridDatas[stageId].saveDatas[saveIdx];
        }
        public StageResultData GetClientResultData(int stageId)
        {
            return gameplayData.stageGridDatas[stageId].resultDatas;
        }

        public void AddStageGridData(int stageId, int saveIdx, StageSaveData datas)
        {
            gameplayData.stageGridDatas[stageId].saveDatas[saveIdx] = datas;
        }

        public Action OnSaveClearStage { get; set; }
		public void SaveClearStage(int stageId, StageResultData data)
        {
            Debug.Log($"Stage: {stageId} Clear! {data.ToString()}");
            gameplayData.stageGridDatas[stageId].resultDatas.UpdateData(data);
            OnSaveClearStage.Invoke();
        }

        /// <summary>
        /// 서버에서 받은 결과 데이터를 저장
        /// 그래프를 그리는데 사용
        /// </summary>
        public void SaveServerResults(ServerResultData data)
		{
			string json = JsonUtility.ToJson(data, true);
			File.WriteAllText(serverResultPath, json);
		}

        /// <summary>
        /// Stage에 해당하는 결과들을 받아옵니다.
        /// </summary>
        public CountResultData GetServerResults(int stageIdx)
        {
            string json = File.ReadAllText(serverResultPath);
			ServerResultData data = JsonUtility.FromJson<ServerResultData>(json);
            if (data.datas.Length <= stageIdx)
            {
                return new CountResultData();
            }
            return data.datas[stageIdx];
        }

        // 나머지 LanguageIndex, IsRevealBlood 는 필요할때 프로퍼티로 갖다 쓰시면 됩니다.
        private void ApplyBasicSettings()
        {
            Managers.Sound.BgmVolume = settingData.BgmVolume;
            Managers.Sound.SfxVolume = settingData.SfxVolume;
            Managers.Sound.MasterVolume = settingData.MasterVolume;
        }

        private void InitSettingData()
        {
            if (settingData.KeyBindings == null)
			{
				settingData.KeyBindings = new int[Enum.GetValues(typeof(ShortcutActionEnum)).Length-1];
                for (int i = 0; i < settingData.KeyBindings.Length; i++)
				{
					settingData.KeyBindings[i] = (int)Constants.KEYCODE_SHORTCUT_DEFAULT[i];
                }
            }
        }
        public void ChangeBinding(ShortcutActionEnum selectedAction, int keycode)
        {
            for (int i = 0; i < settingData.KeyBindings.Length; i++)
            {
                if (settingData.KeyBindings[i] == keycode)
                {
                    settingData.KeyBindings[i] = 0;
                }
            }
            settingData.KeyBindings[(int)selectedAction] = keycode;
            OnUpdateKeyBindings.Invoke();
        }

        public void UpdateBasicSettingChanges()
        {
            OnUpdateBasicSettings.Invoke(settingData);
        }

        public bool IsAbleToAccessStage(int idx)
		{

#if UNITY_EDITOR
            if (isUnlockAll) return true;
#endif

			int preIdx = Managers.Resource.GetStageInfo(idx).prerequisite;
            return preIdx < 0 ? 
                true : gameplayData.stageGridDatas[preIdx].resultDatas.CycleCount >= 0;
        }

#if UNITY_EDITOR
        bool isUnlockAll = false;
        public void UnlockAll()
        {
            isUnlockAll = true;
        }
#endif

    }
}