
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

            OnMainScene();
            OnMenuScene();
            OnGameScene();
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
        }


        private ClickableScreenBase prevScreen = null;
        private void InteractClickableObject()
        {
            if (Physics.Raycast(cameraRay, out RaycastHit hit, 20, Constants.LAYER_CLICKABLE)
                && IsMouseInputEnabled())
            {
                prevScreen = hit.transform.GetComponent<ClickableScreenBase>();
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
                Camera.main.GetComponent<CameraBase>().LerpToOrigin();
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