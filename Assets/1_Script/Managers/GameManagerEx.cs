using HumanFactory.Props;
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

        private void Start()
        {
            
        }

        private void Update()
        {
            ShootRayCast();
        }

        public void ChangeRenderCamera(CameraType type)
        {
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
            lockRayCast = false;
        }

        private bool lockRayCast = false;
        private ClickableScreenBase prevScreen = null;
        private void ShootRayCast()
        {
            if (lockRayCast) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 20) 
                && hit.transform.GetComponent<ClickableScreenBase>() != null)
            {
                prevScreen = hit.transform.GetComponent<ClickableScreenBase>();
                prevScreen?.OnPointerEnter();

                // INPUT - Mouse 0
                if(Input.GetMouseButtonDown(0))
                {
                    lockRayCast = true;
                    prevScreen?.OnPointerClick();
                }
            }
            else
            {
                prevScreen?.OnPointerExit();
            }
        }
    }
}

