using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
	public class SettingPanelUI : MonoBehaviour
	{
		[SerializeField] private List<Button> settingButtons = new List<Button>();
		[SerializeField] private List<Transform> settingPanels = new List<Transform>();

		private void Start()
		{
			for (int i = 0; i < settingButtons.Count; i++)
			{
				int t = i;
				settingButtons[i].onClick.AddListener(() =>
				{
					OnButtonClick(t);
				});
			}

			OnButtonClick(0);
		}

		private void OnButtonClick(int index)
		{
			for (int i = 0; i < settingButtons.Count; i++)
			{
				settingButtons[i].GetComponent<UIItemBase>().OnSelected(i == index);
				if (i == index)
				{
					settingPanels[i].gameObject.SetActive(true);
				}
				else
				{
					settingPanels[i].gameObject.SetActive(false);
				}
			}
		}

	}
}