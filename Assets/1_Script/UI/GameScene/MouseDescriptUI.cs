using HumanFactory.Manager;
using TMPro;
using UnityEngine;

namespace HumanFactory.UI
{
    public class MouseDescriptUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI LeftMouseText;
        [SerializeField] private TextMeshProUGUI RightMouseText;

        private void Start()
        {
            Managers.Input.OnModeChangedAction -= OnModeChanged;
            Managers.Input.OnModeChangedAction += OnModeChanged;

            Managers.Input.OnHoverInNoneModeAction -= OnHoverInNoneMode;
            Managers.Input.OnHoverInNoneModeAction += OnHoverInNoneMode;
        }

        private void OnHoverInNoneMode(bool isCircuiting, BuildingType type)
        {
            if (isCircuiting)
            {
                LeftMouseText.text = "Set Target";
                RightMouseText.text = "";
                return;
            }

            switch (type)
            {
                case BuildingType.ToggleButton:
                    LeftMouseText.text = "Select Button";
                    RightMouseText.text = "";
                    break;
                case BuildingType.RotateButton:
                    LeftMouseText.text = "Select Button";
                    RightMouseText.text = "Rotate";
                    break;
                case BuildingType.Jump:
                    LeftMouseText.text = "Select Button";
                    RightMouseText.text = "Toggle ON/OFF";
                    break;
                case BuildingType.Sub1:
                    LeftMouseText.text = "Select Button";
                    RightMouseText.text = "Toggle ON/OFF";
                    break;
                case BuildingType.Add1:
                    LeftMouseText.text = "Select Button";
                    RightMouseText.text = "Toggle ON/OFF";
                    break;
                case BuildingType.Button:
                    LeftMouseText.text = "Select Button";
                    RightMouseText.text = "Toggle ON/OFF";
                    break;
                case BuildingType.Double:
                    LeftMouseText.text = "Select Button";
                    RightMouseText.text = "Toggle ON/OFF";
                    break;
                default:
                    LeftMouseText.text = "Select Button";
                    RightMouseText.text = "";
                    break;
            }
        }

        private void OnModeChanged(InputMode mode)
        {
            switch (mode)
            {
                case InputMode.None:
                    LeftMouseText.text = "Select";
                    RightMouseText.text = "";
                    break;
                case InputMode.Pad:
                    LeftMouseText.text = "Set Direction";
                    RightMouseText.text = "Clear Direction";
                    break;
                case InputMode.Building:
                    LeftMouseText.text = "Set Button";
                    RightMouseText.text = "Remove Button";
                    break;
            }
            return;
        }

    }
}