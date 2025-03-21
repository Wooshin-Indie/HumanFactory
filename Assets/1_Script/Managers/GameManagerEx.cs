using HumanFactory.Manager;
using HumanFactory.Server;
using HumanFactory.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace HumanFactory
{
	public class GameManagerEx : MonoBehaviour
    {
        #region Singleton
        static GameManagerEx s_instance;
        public static GameManagerEx Instance { get { return s_instance; } }

        public void Init()
        {
            //Managers.Input.LockMouseInput();
            if (s_instance == null)
            {
                s_instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
        }

        private void Awake()
        {
            Init();

			Cursor.SetCursor(mouseTexture, new Vector2(0, 0),CursorMode.ForceSoftware);
		}

		private void Start()
		{
			ConvertUIRaycaster(CameraType.Main);
		}
		#endregion

		// Camera : Main, Menu, Game 순
		[SerializeField] private List<Camera> cameras;
        [SerializeField] private List<RenderTexture> renderTextures;

        [SerializeField] private Texture2D mouseTexture;
        public List<Camera> Cameras { get { return cameras; } }

        [SerializeField] private Material scanlineMat;

        private CameraType currentCamType = CameraType.Main;
        public CameraType CurrentCamType { get => currentCamType; }

        /// <summary>
        /// 게임씬을 출력하는 카메라를 변경합니다.
        /// CCTV를 구현하기 위한 RenderTexture를 끊고
        /// Tag를 mainCamera로 변경해야합니다.
        /// </summary>
        public void ChangeRenderCamera(CameraType type)
        {
            currentCamType = type;
            switch (type)
            {
                case CameraType.Main:
                    cameras[0].gameObject.SetActive(true);
                    cameras[1].gameObject.SetActive(true);
                    cameras[3].gameObject.SetActive(true);
                    cameras[1].targetTexture = renderTextures[0];
                    cameras[2].targetTexture = renderTextures[1];
                    cameras[3].targetTexture = renderTextures[2];
                    cameras[0].tag = Constants.TAG_CAMERA;
                    cameras[1].tag = Constants.TAG_NONE;
                    cameras[2].tag = Constants.TAG_NONE;
                    cameras[3].tag = Constants.TAG_NONE;
                    break;
                case CameraType.Menu:
                    cameras[0].gameObject.SetActive(false);
                    cameras[1].gameObject.SetActive(true);
                    cameras[3].gameObject.SetActive(false);
                    cameras[1].targetTexture = null;
                    cameras[2].targetTexture = renderTextures[1];
                    cameras[0].tag = Constants.TAG_NONE;
                    cameras[1].tag = Constants.TAG_CAMERA;
                    cameras[2].tag = Constants.TAG_NONE;
                    cameras[3].tag = Constants.TAG_NONE;
                    break;
                case CameraType.Game:
                    cameras[0].gameObject.SetActive(false);
                    cameras[1].gameObject.SetActive(false);
                    cameras[3].gameObject.SetActive(false);
                    cameras[2].targetTexture = null;
                    cameras[0].tag = Constants.TAG_NONE;
                    cameras[1].tag = Constants.TAG_NONE;
                    cameras[2].tag = Constants.TAG_CAMERA;
                    cameras[3].tag = Constants.TAG_NONE;
                    break;
                case CameraType.Setting:
                    cameras[0].gameObject.SetActive(false);
                    cameras[3].targetTexture = null;
                    cameras[0].tag = Constants.TAG_NONE;
                    cameras[1].tag = Constants.TAG_NONE;
                    cameras[2].tag = Constants.TAG_NONE;
                    cameras[3].tag = Constants.TAG_CAMERA;
                    break;
            }

            ConvertUIRaycaster(type);
        }

        [SerializeField]
        private List<GraphicRaycaster> raycasters
            = new List<GraphicRaycaster>(Enum.GetNames(typeof(CameraType)).Length);

        public List<GraphicRaycaster> RayCasters { get { return raycasters; } }

        private void ConvertUIRaycaster(CameraType type)
		{
			SetScanlineMaterial(type != CameraType.Main);

			for (int i = 0; i < raycasters.Count; i++)
            {
                if (raycasters[i] == null) continue;

                if (i == (int)type)
                {

                    raycasters[i].enabled = true;
                }
                else raycasters[i].enabled = false;
            }
        }

        /// <summary>
        /// Scanline Shader Graph Toggle 함수
        /// </summary>
        public void SetScanlineMaterial(bool isActive)
        {
            if(!Managers.Data.BasicSettingData.isScanline)
			{
				scanlineMat.SetInt("_IsActive", (isActive) ? 0 : 0);
                return;
			}
            scanlineMat.SetInt("_IsActive", (isActive) ? 1 : 0);
        }

        #region Success/Fail

        [SerializeField] private SuccessPopupUI successUI;


        public void OnStageSuccess(GameResultInfo info)
		{
            if (!Application.isBatchMode)
			{
				BlockAllUIs();
				successUI.gameObject.SetActive(true);
				successUI.SetSuccessPopupInfos(info);
				Managers.Data.SaveClearStage(info.StageIdx, info.ResultData);
			}
            else
            {
                ServerManager.Instance.ServerSimulator.OnSimulationEnd(info, true);
            }
        }

        public void OnStageFail(GameResultInfo info)
        {
            if (!Application.isBatchMode)
            {
                // 배치모드가 아니면 Fail할 경우가 없음
                return;
            }
            else
			{
				ServerManager.Instance.ServerSimulator.OnSimulationEnd(info, false);
			}
        }


        #endregion

        [SerializeField] private ExecuteType exeType = ExecuteType.None;
        public ExecuteType ExeType { get => exeType; }
        public Action<ExecuteType> OnExeTypeChange { get; set; }

        public void SetExeType(ExecuteType type)
        {
            
            Managers.Input.OnInputModeChanged(InputMode.None);
            switch (type)
            {
                case ExecuteType.None:
                    if (exeType == ExecuteType.None) break;
                    Managers.Input.ReleaseMouseInput();
                    if (exeType == ExecuteType.Pause) break;
                    MapManager.Instance.LockCycle();
                    break;
                case ExecuteType.Play:
                    if (exeType == ExecuteType.Play) break;
                    MapManager.Instance.ReleaseCycle();
                    if (exeType == ExecuteType.Pause) break;
                    Managers.Input.LockMouseInput();
                    break;
                case ExecuteType.Pause:
                    if (exeType == ExecuteType.Pause) break;
                    if (exeType == ExecuteType.None)
					{
						Managers.Input.LockMouseInput();
						break;
                    }
					MapManager.Instance.LockCycle();
                    if (exeType == ExecuteType.Play) break;
                    Managers.Input.LockMouseInput();
                    break;
            }
            exeType = type;
            OnExeTypeChange?.Invoke(type);
		}

        public void BlockAllUIs()
		{
			for (int i = 0; i < raycasters.Count; i++)
				raycasters[i].enabled = false;
		}

#if UNITY_EDITOR
        [ContextMenu("Unlock All Stages")]
        public void UnlockAllStages()
        {
            Managers.Data.UnlockAll();
        }
#endif

        [Header("LOGs")]
        [SerializeField] private LogPanelUI logUI;

        HashSet<KeyValuePair<string, Color>> logKeys = new HashSet<KeyValuePair<string, Color>>();
		public void DisplayLogByKey(string key, Color color)
		{
            logKeys.Add(new KeyValuePair<string, Color>(key, color));
		}

        private void OnUpdateLog()
        {
            if (logKeys.Count == 0) return;

            foreach(var logkey in  logKeys)
            {
				logUI.DisplayLog(LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_LOG, logkey.Key), logkey.Value);
			}
            logKeys.Clear();
        }

		private void Update()
		{
            OnUpdateLog();
        }

        [SerializeField] private ESCPanel escPanel;
        public ESCPanel ESCPanel { get => escPanel; }
        public void OpenESCPanel()
        {
            escPanel.gameObject.SetActive(true);
        }

	}
}

 