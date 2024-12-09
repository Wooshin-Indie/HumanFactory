
using HumanFactory.Props;
using UnityEngine;

namespace HumanFactory.Manager
{
    public class InputManager
    {
        private Ray cameraRay;
        public void OnUpdate()
        {
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // HACK - SoundManager 테스트용 입력입니다.
            // 나중에 UI에서 호출하도록 변경해야합니다.
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Managers.Sound.ChangeBGM(true);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Managers.Sound.ChangeBGM(false);
            }

            OnMainScene();
            OnMenuScene();
            OnGameScene();
            OnSettingScene();
        }

        private int mouseInputLock = 0;
        public void LockMouseInput()
        {
            mouseInputLock++;
        }
        public void ReleaseMouseInput()
        {
            if (mouseInputLock > 0)
                mouseInputLock--;
            else
                Debug.LogError("Error : Release Mouse Input dosen't expected!");
        }
        private bool IsMouseInputEnabled()
        {
            return mouseInputLock <= 0;
        }

        private Vector3 worldPos;
        private Vector2Int curMousePos;
        private void OnGameScene()
        {
            if (GameManagerEx.Instance.CurrentCamType != CameraType.Game) return;
            
            ClickMapGrid();
            ClickOutScene();
        }

        private void OnMenuScene()
        {
            if (GameManagerEx.Instance.CurrentCamType != CameraType.Menu) return;

            InteractClickableObject();
            ClickOutScene();
        }

        private void OnMainScene()
        {
            if (GameManagerEx.Instance.CurrentCamType != CameraType.Main) return;

            InteractClickableObject();

            if (Camera.main.GetComponent<CameraBase>().IsZoomed)
            {
                ClickOutScene();
            }
        }

        private void OnSettingScene()
        {
            if (GameManagerEx.Instance.CurrentCamType != CameraType.Setting) return;

            ClickOutScene();
        }

        private ClickableBase prevScreen = null;
        private void InteractClickableObject()
        {
            if (Camera.main.GetComponent<CameraBase>().IsZoomed) return;

            if (Physics.Raycast(cameraRay, out RaycastHit hit, 20, Constants.LAYER_CLICKABLE)
                && IsMouseInputEnabled())
            {
                prevScreen = hit.transform.GetComponent<ClickableBase>();
                prevScreen?.OnPointerEnter();

                if (Input.GetMouseButtonDown(0))
                {
                    LockMouseInput();
                    prevScreen?.OnPointerClick();
                }
            }
            else
            {
                prevScreen?.OnPointerExit();
            }
        }

        // HACK - 임시로 테스트하기 위한 함수
        private void ClickOutScene()
        {
            if (!IsMouseInputEnabled()) return;
            if (Input.GetMouseButtonDown(1))
            {
                LockMouseInput();
                Camera.main.GetComponent<CameraBase>().ConvertSceneBackward();
            }
        }

        /// <summary>
        /// GameScene - Layer 1 일떄만 입력을 받아야됨
        /// </summary>
        private void ClickMapGrid()
        {
            if (!IsMouseInputEnabled()) return;
            if (Input.GetMouseButtonDown(0))
            {
                worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                curMousePos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));

                MapManager.Instance.OnClickMapGrid(curMousePos.x, curMousePos.y);
            }
        }

    }

}