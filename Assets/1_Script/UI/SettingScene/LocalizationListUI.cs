using HumanFactory.Manager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class LocalizationListUI : MonoBehaviour
    {
        [SerializeField] private List<UIItemBase> items = new List<UIItemBase>();

		private void Start()
		{
			int startIdx = LocalizationSettings.AvailableLocales.Locales.IndexOf(
				LocalizationSettings.SelectedLocale
				);
			Managers.Data.BasicSettingData.languageIndex = startIdx;
			for (int i = 0; i < items.Count; i++)
			{
				items[i].OnSelected(i == startIdx);
			}
			LocalizationSettings.SelectedLocale =
				LocalizationSettings.AvailableLocales.Locales[startIdx];

			for (int i = 0; i < items.Count; i++)
			{
				int t = i;
				items[i].GetComponent<Button>().onClick.AddListener(() =>
					{
						OnClickItem(t);
					});
			}
		}

		private void OnClickItem(int idx)
		{
			Managers.Sound.PlaySfx(SFXType.UI_Click, .3f, .95f);
			Managers.Data.BasicSettingData.languageIndex = idx;
			for (int i = 0; i < items.Count; i++)
            {
                items[i].OnSelected(i == idx);
			}
			LocalizationSettings.SelectedLocale =
				LocalizationSettings.AvailableLocales.Locales[idx];
		}
    }
}