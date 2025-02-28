using HumanFactory.Manager;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace HumanFactory.UI
{
    public class MouseDescriptUI : MonoBehaviour
    {
        [SerializeField] private GameObject DragDescript;

        [SerializeField] private TextMeshProUGUI LeftMouseText;
        [SerializeField] private TextMeshProUGUI DragMouseText;
        [SerializeField] private TextMeshProUGUI RightMouseText;

        private void Start()
        {
            Managers.Input.OnModeChangedAction -= OnModeChanged;
            Managers.Input.OnModeChangedAction += OnModeChanged;

            Managers.Input.OnHoverInNoneModeAction -= OnHoverInNoneMode;
            Managers.Input.OnHoverInNoneModeAction += OnHoverInNoneMode;

            Managers.Input.OnBuildingTypeChanged -= OnBuildingTypeChanged;
            Managers.Input.OnBuildingTypeChanged += OnBuildingTypeChanged;

            DragDescript.gameObject.SetActive(true);
        }

        private string prevLeftKey = "";
        private string prevDragKey = "";
        private string prevRightKey = "";
        private void OnHoverInNoneMode(bool isCircuiting, BuildingType type)
        {
            string leftKey = "Empty";
            string dragKey = "Empty";
            string rightKey = "Empty";


            if (isCircuiting)
            {
                leftKey = "MouseCircuitLeft";
                rightKey = "MouseCircuitRight";
            }
            else
            {
                switch (type)
                {
                    case BuildingType.None:
                        leftKey = "MouseNoneLeft";
                        if (!DragDescript.gameObject.activeSelf)
                            DragDescript.gameObject.SetActive(true);
                        dragKey = "MouseNoneDrag";
                        rightKey = "Empty";
                        break;
                    case BuildingType.Toggle:
                        leftKey = "MouseNoneLeft";
                        if (!DragDescript.gameObject.activeSelf)
                            DragDescript.gameObject.SetActive(true);
                        dragKey = "MouseNoneDrag";
                        rightKey = "Empty";
                        break;
                    case BuildingType.Rotate:
                        leftKey = "MouseNoneLeft";
                        if (!DragDescript.gameObject.activeSelf)
                            DragDescript.gameObject.SetActive(true);
                        dragKey = "MouseNoneDrag";
                        rightKey = "MouseNoneRight0";
                        break;
                    case BuildingType.Jump:
                    case BuildingType.Sub:
                    case BuildingType.Add:
                    case BuildingType.NewInput:
                        leftKey = "Empty";
                        if (!DragDescript.gameObject.activeSelf)
                            DragDescript.gameObject.SetActive(true);
                        dragKey = "MouseNoneDrag";
                        rightKey = "MouseNoneRight1";
                        break;
                    case BuildingType.Double:
                        leftKey = "MouseNoneRight0";
                        if (!DragDescript.gameObject.activeSelf)
                            DragDescript.gameObject.SetActive(true);
                        dragKey = "MouseNoneDrag";
                        rightKey = "MouseNoneRight1";
                        break;
                    default:
                        leftKey = "MouseNoneLeft";
                        if (!DragDescript.gameObject.activeSelf)
                            DragDescript.gameObject.SetActive(true);
                        dragKey = "MouseNoneDrag";
                        rightKey = "Empty";
                        break;
                }
            }

            if (leftKey != prevLeftKey)
            {
                LeftMouseText.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
                {
                    TableReference = Constants.TABLE_GAMEUI,
                    TableEntryReference = leftKey
                };

                prevLeftKey = leftKey;
            }

                DragMouseText.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
                {
                    TableReference = Constants.TABLE_GAMEUI,
                    TableEntryReference = dragKey
                };
                prevDragKey = dragKey;

            if (rightKey != prevRightKey)
            {
                RightMouseText.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
                {
                    TableReference = Constants.TABLE_GAMEUI,
                    TableEntryReference = rightKey
                };
                prevRightKey = rightKey;
            }
        }

        private void OnModeChanged(InputMode mode)
        {
            string leftKey = "Empty";
            string dragKey = "Empty";
            string rightKey = "Empty";

            switch (mode)
            {
                case InputMode.None:
                    leftKey = "MouseNoneLeft";
                    if (!DragDescript.gameObject.activeSelf)
                        DragDescript.gameObject.SetActive(true);
                    dragKey = "MouseNoneDrag";
                    rightKey = "Empty";
                    break;
                case InputMode.Pad:
                    leftKey = "MousePadLeft";
                    if (!DragDescript.gameObject.activeSelf)
                        DragDescript.gameObject.SetActive(true);
                    dragKey = "MousePadDrag";
                    rightKey = "MousePadRight";
                    break;
                case InputMode.Building:
                    leftKey = "MouseBuildingLeft";
                    if (DragDescript.gameObject.activeSelf)
                        DragDescript.gameObject.SetActive(false);
                    rightKey = "MouseBuildingRight";
                    break;
            }


            LeftMouseText.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
            {
                TableReference = Constants.TABLE_GAMEUI,
                TableEntryReference = leftKey
            };
            DragMouseText.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
            {
                TableReference = Constants.TABLE_GAMEUI,
                TableEntryReference = dragKey
            };
            RightMouseText.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
            {
                TableReference = Constants.TABLE_GAMEUI,
                TableEntryReference = rightKey
            };
            return;
        }

        private void OnBuildingTypeChanged(BuildingType type)
        {
            string leftKey = "Empty";
            string dragKey = "Empty";
            string rightKey = "Empty";

            if (Managers.Input.InputMode != InputMode.Building) return;
            switch (type)
            {
                case BuildingType.None:
                    leftKey = "MouseBuildingLeft_None";
                    if (DragDescript.gameObject.activeSelf)
                        DragDescript.gameObject.SetActive(false);
                    rightKey = "Empty";
                    break;
                default:
                    leftKey = "MouseBuildingLeft";
                    if (DragDescript.gameObject.activeSelf)
                        DragDescript.gameObject.SetActive(false);
                    rightKey = "MouseBuildingRight";
                    break;
            }

            LeftMouseText.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
            {
                TableReference = Constants.TABLE_GAMEUI,
                TableEntryReference = leftKey
            };
            DragMouseText.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
            {
                TableReference = Constants.TABLE_GAMEUI,
                TableEntryReference = dragKey
            };
            RightMouseText.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
            {
                TableReference = Constants.TABLE_GAMEUI,
                TableEntryReference = rightKey
            };
        }
    }
}