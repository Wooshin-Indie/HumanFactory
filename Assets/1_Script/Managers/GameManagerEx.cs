using System.Collections.Generic;
using UnityEngine;

namespace HumanFactory
{
    public class GameManagerEx : MonoBehaviour
    {
        #region Singleton
        static GameManagerEx s_instance;
        public static GameManagerEx Instance { get { return s_instance; } }

        public void Init()
        {
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
        }
        #endregion

        // Camera : Main, Menu, Game 순
        [SerializeField] private List<Camera> cameras;
        [SerializeField] private List<RenderTexture> renderTextures;

        public List<Camera> Cameras { get { return cameras; } }


        private CameraType currentCamType = CameraType.Main;
        public CameraType CurrentCamType { get => currentCamType; }

        /// <summary>
        /// 게임씬을 출력하는 카메라를 변경합니다.
        /// CCTV를 구현하기 위한 RenderTexture를 끊고
        /// Tag를 mainCamera로 변경해야합니다.
        /// </summary>
        /// <param name="type"></param>
        public void ChangeRenderCamera(CameraType type)
        {
            currentCamType = type;
            switch (type) {
                case CameraType.Main:
                    cameras[0].gameObject.SetActive(true);
                    cameras[1].gameObject.SetActive(true);
                    cameras[1].targetTexture = renderTextures[0];
                    cameras[2].targetTexture = renderTextures[1];
                    cameras[0].tag = Constants.TAG_CAMERA;
                    cameras[1].tag = Constants.TAG_NONE;
                    cameras[2].tag = Constants.TAG_NONE;
                    break;
                case CameraType.Menu:
                    cameras[0].gameObject.SetActive(false);
                    cameras[1].gameObject.SetActive(true);
                    cameras[1].targetTexture = null;
                    cameras[2].targetTexture = renderTextures[1];
                    cameras[0].tag = Constants.TAG_NONE;
                    cameras[1].tag = Constants.TAG_CAMERA;
                    cameras[2].tag = Constants.TAG_NONE;
                    break;
                case CameraType.Game:
                    cameras[0].gameObject.SetActive(false);
                    cameras[1].gameObject.SetActive(false);
                    cameras[2].targetTexture = null;
                    cameras[0].tag = Constants.TAG_NONE;
                    cameras[1].tag = Constants.TAG_NONE;
                    cameras[2].tag = Constants.TAG_CAMERA;
                    break;
            }
        }

    }
}

