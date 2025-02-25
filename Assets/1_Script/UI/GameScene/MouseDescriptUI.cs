using HumanFactory.Manager;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace HumanFactory.UI
{
    public class MouseDescriptUI : MonoBehaviour
    {
        //										left, drag, right
        [SerializeField] private List<GameObject> MouseDescripts = new List<GameObject>();

        [SerializeField] private TextMeshProUGUI LeftMouseText;
        [SerializeField] private TextMeshProUGUI DragMouseText;
        [SerializeField] private TextMeshProUGUI RightMouseText;

        private void Start()
        {
            Managers.Input.OnModeChangedAction -= OnModeChanged;
            Managers.Input.OnModeChangedAction += OnModeChanged;

            Managers.Input.OnHoverInNoneModeAction -= OnHoverInNoneMode;
            Managers.Input.OnHoverInNoneModeAction += OnHoverInNoneMode;

            //MouseDescripts[1].gameObject.SetActive(false);
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
						leftKey = "Clear Button";
						//if (MouseDescripts[1].gameObject.activeSelf)
							//MouseDescripts[1].gameObject.SetActive(false);
                        break;
					case BuildingType.Toggle:
						leftKey = "MouseNoneLeft";
						rightKey = "Empty";
						break;
					case BuildingType.Rotate:
						leftKey = "MouseNoneLeft";
						rightKey = "MouseNoneRight0";
						break;
					case BuildingType.Jump:
					case BuildingType.Sub:
					case BuildingType.Add:
					case BuildingType.NewInput:
						leftKey = "Empty";
						rightKey = "MouseNoneRight1";
						break;
					case BuildingType.Double:
						leftKey = "MouseNoneRight0";
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
			string dragKey = "Empty";
			string rightKey = "Empty";

			switch (mode)
            {
                case InputMode.None:
					leftKey = "MouseNoneLeft";
                    if (!MouseDescripts[1].gameObject.activeSelf)
                        MouseDescripts[1].gameObject.SetActive(true);
                    dragKey = "MouseNoneDrag";
                    rightKey = "Empty";
                    break;
                case InputMode.Pad:
					leftKey = "MousePadLeft";
					if (!MouseDescripts[1].gameObject.activeSelf)
                        MouseDescripts[1].gameObject.SetActive(true);
                    dragKey = "MouseNoneDrag";
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