using HumanFactory.Props;
using System;
using System.Linq;
using Unity.VisualScripting;
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
            worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            curMousePos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));

            OnMainScene();
            OnMenuScene();
            OnGameScene();
            OnSettingScene();

        }

        #region MouseInputLock
        private int mouseInputLock = 0;
        public void LockMouseInput()
        {
            Debug.Log("MOUSE LOCK");
            mouseInputLock++;
        }
        public void ReleaseMouseInput()
        {
            Debug.Log("MOUSE RELEASE");
            if (mouseInputLock > 0)
                mouseInputLock--;
            else
                Debug.LogError("Error : Release Mouse Input dosen't expected!");
        }
        public bool IsMouseInputEnabled()
		{
			// 게임 실행중이라 락걸림
			if (mouseInputLock > 0 && GameManagerEx.Instance.ExeType != ExecuteType.None)
			{
				for (int i = (int)ShortcutActionEnum.Add_Button; i <= (int)ShortcutActionEnum.Back; i++)
				{
					if (Input.GetKeyDown((KeyCode)Managers.Data.BasicSettingData.keyBindings[i]))
					{
						GameManagerEx.Instance.DisplayLogByKey("Lock_1", Color.red);
					}
				}
				if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    if(MapManager.Instance.CheckBoundary(curMousePos.x, curMousePos.y, MapManager.Instance.IsMapExpanded))
					{
						GameManagerEx.Instance.DisplayLogByKey("Lock_1", Color.red);
					}
				}
            }

            return mouseInputLock <= 0;
        }

        private int menuInputLock = 0;
        public void LockMenuInput()
        {
            menuInputLock++;
        }
        public void ReleaseMenuInput()
        {
            if (menuInputLock > 0)
                menuInputLock--;
            else
                Debug.LogError("Error : Release Mouse Input dosen't expected!");
        }

        private bool IsMenuInputEnabled()
        {
            return menuInputLock <= 0;
        }
        #endregion

        private Vector3 worldPos;
        private Vector2Int curMousePos;
        private void OnMenuScene()
        {
            if (GameManagerEx.Instance.CurrentCamType != CameraType.Menu) return;

            if (IsMenuInputEnabled())
                InteractClickableObject();
            ClickOutScene();
        }

        private void OnMainScene()
        {
            if (GameManagerEx.Instance.CurrentCamType != CameraType.Main) return;

            InteractClickableObject();

            ClickOutScene();
        }

        private void OnGameScene()
        {
            if (GameManagerEx.Instance.CurrentCamType != CameraType.Game) return;

            ChangeInputMode();

            OnGameSceneNoneMode();
            OnGameScenePadMode();
            OnGameSceneBuildingMode();
            ShortcutBuilding();

            ClickOutScene();
        }

        // UI 켜지는 함수 넣어둬야됨
        public Action<Vector2Int> OnClickMapGridInNoneModeAction { get; set; }
        public Action<bool, BuildingType> OnHoverInNoneModeAction { get; set; }
        Vector2Int prevMousePos = new Vector2Int(-1, -1);

        private void OnGameSceneNoneMode()
        {
            if (inputMode != InputMode.None) return;

            BuildingType type = MapManager.Instance.OnHoverMapGridInNoneMode(curMousePos.x, curMousePos.y);
            OnHoverInNoneModeAction?.Invoke(MapManager.Instance.IsCircuiting, type);

            if (!IsMouseInputEnabled()) return;


            if (Input.GetMouseButtonDown(0))
            {
                prevMousePos = new Vector2Int(curMousePos.x, curMousePos.y);
                MapManager.Instance.OnDragStart(prevMousePos);
            }
            else if (Input.GetMouseButton(0))
            {
                MapManager.Instance.OnDrag(new Vector2(worldPos.x, worldPos.y));
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (MapManager.Instance.IsDragging)
                {
                    MapManager.Instance.OnDragEnd(prevMousePos, curMousePos);
                }
                else
                {
                    MapManager.Instance.OnLeftClickMapGridInNoneMode(curMousePos.x, curMousePos.y, !MapManager.Instance.IsCircuiting);
                }
                MapManager.Instance.OnClearDrag();
                prevMousePos = new Vector2Int(-1, -1);
            }

            if (Input.GetMouseButtonDown(1))
            {
                MapManager.Instance.OnRightClickMapGridInNoneMode(curMousePos.x, curMousePos.y);
            }
        }

        private void OnGameScenePadMode()
        {
            if (inputMode != InputMode.Pad || !IsMouseInputEnabled()) return;

            ClickMapGridInPadMode();
        }

        private void OnGameSceneBuildingMode()
        {
            if (inputMode != InputMode.Building || !IsMouseInputEnabled()) return;

            MapManager.Instance.OnHoverMapGridInBuildingMode(curMousePos.x, curMousePos.y, currentSelectedBuilding);
            ClickMapGridInBuildingMode();
        }

        private void OnSettingScene()
        {
            if (GameManagerEx.Instance.CurrentCamType != CameraType.Setting) return;

            ClickOutScene();
            OnBindingKey();
        }

        /* Input Modules */

        private ClickableBase lastScreen = null;
        private ClickableBase prevScreen = null;
        private void InteractClickableObject()
        {
            if (Camera.main.GetComponent<CameraBase>().IsZoomed) return;

            if (Physics.Raycast(cameraRay, out RaycastHit hit, 20, Constants.LAYER_CLICKABLE)
                && IsMouseInputEnabled())
            {
                prevScreen = hit.transform.GetComponent<ClickableBase>();

                if (lastScreen == null)
                {
                    prevScreen?.OnPointerEnter();
                    lastScreen = prevScreen;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    prevScreen?.OnPointerClick();
                }
            }
            else
            {
                prevScreen?.OnPointerExit();
                lastScreen = prevScreen = null;
            }
        }

        private void ClickOutScene()
        {
            if (Input.GetKeyDown((KeyCode)Managers.Data.BasicSettingData.keyBindings[(int)ShortcutActionEnum.Back]))
            {
                OnEscape();
            }
        }

        public void OnEscape()
        {
            if (!IsMouseInputEnabled()) return;
            Managers.Sound.PlaySfx(SFXType.Back, 0.8f, 1.7f);
            LockMouseInput();

            /** SAVE SETTINGS **/
            if (GameManagerEx.Instance.CurrentCamType == CameraType.Game) MapManager.Instance.SaveStage();
            else if (GameManagerEx.Instance.CurrentCamType == CameraType.Main ||
                GameManagerEx.Instance.CurrentCamType == CameraType.Setting) Managers.Data.SaveSettingData();

            Camera.main.GetComponent<CameraBase>().ConvertSceneBackward();
            inputMode = InputMode.None;
            OnInputModeChanged(inputMode);
        }

        /// <summary>
        /// GameScene - Layer 1 일떄만 입력을 받아야됨
        /// </summary>
        private void ClickMapGridInPadMode()
        {
            if (!IsMouseInputEnabled()) return;
            if (Input.GetMouseButton(0))
            {
                MapManager.Instance.OnClickMapGridInPadMode(curMousePos.x, curMousePos.y);
            }
            else
            {
                MapManager.Instance.OnReleaseMapGridInPadMode();
            }

            if (Input.GetMouseButton(1))
            {
                MapManager.Instance.OnRightClickMapGridInPadMode(curMousePos.x, curMousePos.y);
            }

        }

        private BuildingType currentSelectedBuilding = BuildingType.None;
        public Action<BuildingType> OnBuildingTypeChanged { get; set; }

        public void ChangeCurSelectedBuilding(BuildingType type)
        {
            if (MapManager.Instance.CurrentStageInfo == null) return;
            if (!MapManager.Instance.CurrentStageInfo.enableBuildings.Contains((int)type)
                && type != BuildingType.None) return;

            OnInputModeChanged(InputMode.Building);
            currentSelectedBuilding = type;
            OnBuildingTypeChanged?.Invoke(type);

            Debug.Log($"CurSelectedBuilding: {currentSelectedBuilding}");
        }

        /// <summary>
        /// GameScene - Layer 2 일때 입력을 받음.
        /// 건물 설치
        /// </summary>
        private void ClickMapGridInBuildingMode()
        {
            if (!IsMouseInputEnabled()) return;
            if (Input.GetMouseButtonDown(0))
            {
                //if (currentSelectedBuilding == BuildingType.None) return;
                MapManager.Instance.OnLeftClickMapGridInBuildingMode(curMousePos.x, curMousePos.y, currentSelectedBuilding);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                OnInputModeChanged(InputMode.None);
            }

        }

        private void ShortcutBuilding()
		{
			if (Input.GetKeyDown((KeyCode)Managers.Data.BasicSettingData.keyBindings[(int)ShortcutActionEnum.Zoom_Map]))
			{
				MapManager.Instance.ToggleZoomMap();
			}

			if (!IsMouseInputEnabled()) return;
            for (int i = (int)ShortcutActionEnum.Add_Button; i < (int)ShortcutActionEnum.Back; i++)
            {
                if (Input.GetKeyDown((KeyCode)Managers.Data.BasicSettingData.keyBindings[i]))
                {
                    ChangeCurSelectedBuilding((BuildingType)(i));
				}
            }

        }

        public Action<InputMode> OnModeChangedAction { get; set; }

        private void ChangeInputMode()
        {
            if (!IsMouseInputEnabled()) return;
            if (Input.GetKeyDown((KeyCode)Managers.Data.BasicSettingData.keyBindings[(int)ShortcutActionEnum.ChangeMode_1]) && inputMode != InputMode.None)
            {
                inputMode = InputMode.None;
            }
            else if (Input.GetKeyDown((KeyCode)Managers.Data.BasicSettingData.keyBindings[(int)ShortcutActionEnum.ChangeMode_2]) && inputMode != InputMode.Pad)
            {
                inputMode = InputMode.Pad;
            }
            else { return; }

            OnInputModeChanged(inputMode);
        }

        public void OnInputModeChanged(InputMode mode)
		{
			inputMode = mode;
			OnModeChangedAction?.Invoke(inputMode);
            MapManager.Instance.OnInputModeChanged(inputMode);

            OnBuildingTypeChanged?.Invoke(BuildingType.None);
            currentSelectedBuilding = BuildingType.None;

            if (MapManager.Instance.IsCircuiting)
            {
                MapManager.Instance.OnLeftClickMapGridInNoneMode(-1, -1, false);
            }
        }

        private ShortcutActionEnum selectedKey = ShortcutActionEnum.None;
        private bool isBinding = false;
        private Action OnEndAction { get; set; }

        private void OnBindingKey()
        {
            if (!isBinding) return;

            for (int i = (int)KeyCode.A; i <= (int)KeyCode.Z; i++)
            {
                if (Input.GetKeyDown((KeyCode)i))
                {
                    Managers.Data.ChangeBinding(selectedKey, i);
                    EndBinding();
                    return;
                }
            }
            for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9; i++)
			{
				if (Input.GetKeyDown((KeyCode)i))
				{
					Managers.Data.ChangeBinding(selectedKey, i);
					EndBinding();
					return;
				}
			}

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Managers.Data.ChangeBinding(selectedKey, (int)KeyCode.Escape);
				EndBinding();
				return;
			}
		}

        public void StartBinding(ShortcutActionEnum keyEnum, Action action)
		{
			if (selectedKey == ShortcutActionEnum.None)
				LockMouseInput();
			selectedKey = keyEnum;
            isBinding = true;
            OnEndAction?.Invoke();
            OnEndAction = action;
		}

        private void EndBinding()
		{
			selectedKey = ShortcutActionEnum.None;
			isBinding = false;
            ReleaseMouseInput();
            OnEndAction?.Invoke();
            OnEndAction = null;
        }
	}

}