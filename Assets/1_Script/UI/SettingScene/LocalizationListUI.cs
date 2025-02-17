using HumanFactory.Manager;
using System.Collections.Generic;
using UnityEditor;
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
			OnClickItem(LocalizationSettings.AvailableLocales.Locales.IndexOf(
				LocalizationSettings.SelectedLocale
				));

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
			Managers.Data.BasicSettingData.languageIndex = idx;
			Managers.Sound.PlaySfx(SFXType.UI_Click, .3f, .95f);
			for (int i = 0; i < items.Count; i++)
            {
                items[i].OnSelected(i == idx);
			}
			LocalizationSettings.SelectedLocale =
				LocalizationSettings.AvailableLocales.Locales[idx];
		}
    }
}