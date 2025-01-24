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
			}
			else
			{
                LeftMouseText.text = "Select";
                RightMouseText.text = "Toggle";
            }
		}

		private void OnModeChanged(InputMode mode)
		{
			switch (mode)
			{
				case InputMode.None:
                    LeftMouseText.text = "Select";
                    RightMouseText.text = "Toggle";
                    break;
				case InputMode.Pad:
                    LeftMouseText.text = "Set Direction";
					RightMouseText.text = "Clear Direction";
                    break;
				case InputMode.Building:
                    LeftMouseText.text = "Select";
                    RightMouseText.text = "Toggle";
                    break;
			}
			return;
		}

	}
}