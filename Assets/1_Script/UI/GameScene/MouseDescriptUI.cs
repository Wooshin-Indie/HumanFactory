using HumanFactory.Manager;
using TMPro;
using UnityEngine;

namespace HumanFactory.UI
{
    public class MouseDescriptUI : MonoBehaviour
    {
		[SerializeField] private TextMeshProUGUI tmpText;

		private void Start()
		{
			Managers.Input.OnModeChangedAction -= OnModeChanged;
			Managers.Input.OnModeChangedAction += OnModeChanged;

			Managers.Input.OnHoverInNoneModeAction -= OnHoverInNoneMode;
			Managers.Input.OnHoverInNoneModeAction += OnHoverInNoneMode;
		}

		private void OnHoverInNoneMode(bool isCircuiting, BuildingType type)
		{
			// Debug.Log($"{isCircuiting} , {type}");
		}

		private void OnModeChanged(InputMode mode)
		{
			switch (mode)
			{
				case InputMode.None:
					tmpText.text = "INPUT MODE NONE";
					break;
				case InputMode.Pad:
					tmpText.text = "INPUT MODE PAD";
					break;
				case InputMode.Building:
					tmpText.text = "INPUT MODE BUILDING";
					break;
			}
			return;
		}

	}
}