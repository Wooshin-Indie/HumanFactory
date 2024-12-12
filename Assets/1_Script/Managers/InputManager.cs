using HumanFactory.Props;
using System;
using UnityEngine;

namespace HumanFactory.Manager
{
    public class InputManager
    {
        private InputMode inputMode = InputMode.None;
        private Ray cameraRay;

        public void OnUpdate()
        {
            cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            OnMainScene();
            OnMenuScene();
            OnGameScene();
            OnSettingScene();
        }

        #region MouseInputLock
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
        #endregion

        private Vector3 worldPos;
        private Vector2Int curMousePos;
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

        private void OnGameScene()
        {
            if (GameManagerEx.Instance.CurrentCamType != CameraType.Game) return;

            ChangeInputMode();

            OnGameSceneNoneMode();
            OnGameScenePadMode();
            OnGameSceneBuildingMode();
            OnGameSceneCircuitMode();

            ClickOutScene();
        }

        private void OnGameSceneNoneMode()
        {
            if (inputMode != InputMode.None) return;

        }

        private void OnGameScenePadMode()
        {
            if (inputMode != InputMode.Pad) return;

            ClickMapGrid();
        }

        private void OnGameSceneBuildingMode()
        {
            if (inputMode != InputMode.Building) return;

        }
        private void OnGameSceneCircuitMode()
        {
            if (inputMode != InputMode.Circuit) return;
        }

        private void OnSettingScene()
        {
            if (GameManagerEx.Instance.CurrentCamType != CameraType.Setting) return;

            ClickOutScene();
        }

        /* Input Modules */

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

                inputMode = InputMode.None;
                OnModeChangedAction.Invoke(inputMode);
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

        public Action<InputMode> OnModeChangedAction { get; set; }

        // TODO - GameScene에서 UI 띄우는 함수 필요,
        // none일떄는 시뮬레이션 실행하는 UI 도 띄워줘야됨
        private void ChangeInputMode()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && inputMode != InputMode.None)
            {
                inputMode = InputMode.None;
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2) && inputMode != InputMode.Pad)
            {
                inputMode = InputMode.Pad;
            }
            else if(Input.GetKeyDown(KeyCode.Alpha3) && inputMode != InputMode.Building)
            {
                inputMode = InputMode.Building;
            }
            else if(Input.GetKeyDown(KeyCode.Alpha4) && inputMode != InputMode.Circuit)
            {
                inputMode = InputMode.Circuit;
            }
            else { return; }

            Debug.Log("InputMode Changed to : " + inputMode.ToString());

            OnModeChangedAction.Invoke(inputMode);

        }

    }

}