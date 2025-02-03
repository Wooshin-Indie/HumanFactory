using HumanFactory.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

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

        private string prevLeftKey = "";
        private string prevRightKey = "";
		private void OnHoverInNoneMode(bool isCircuiting, BuildingType type)
		{
            string leftKey = "Empty";
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
					case BuildingType.ToggleButton:
						leftKey = "MouseNoneLeft";
						rightKey = "Empty";
						break;
					case BuildingType.RotateButton:
						leftKey = "MouseNoneLeft";
						rightKey = "MouseNoneRight0";
						break;
					case BuildingType.Jump:
					case BuildingType.Sub1:
					case BuildingType.Add1:
					case BuildingType.Button:
					case BuildingType.Double:
						leftKey = "MouseNoneLeft";
						rightKey = "MouseNoneRight1";
						break;
					default:
						leftKey = "MouseNoneLeft";
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
			string rightKey = "Empty";

			switch (mode)
            {
                case InputMode.None:
					leftKey = "MouseNoneLeft";
					rightKey = "Empty";
                    break;
                case InputMode.Pad:
					leftKey = "MousePadLeft";
					rightKey = "MousePadRight";
                    break;
                case InputMode.Building:
					leftKey = "MouseBuildingLeft";
					rightKey = "MouseBuildingRight";
                    break;
            }


			LeftMouseText.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
			{
				TableReference = Constants.TABLE_GAMEUI,
				TableEntryReference = leftKey
			};
			RightMouseText.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
			{
				TableReference = Constants.TABLE_GAMEUI,
				TableEntryReference = rightKey
			};
			return;
        }

    }
}