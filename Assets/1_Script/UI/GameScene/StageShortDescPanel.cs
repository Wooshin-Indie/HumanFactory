using HumanFactory.Manager;
using HumanFactory.Util;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;

namespace HumanFactory.UI
{

    public class StageShortDescPanel : MonoBehaviour
    {
		private void Start()
		{
			MapManager.Instance.OnCurrentStageIdxAction -= OnLoadStage;
			MapManager.Instance.OnCurrentStageIdxAction += OnLoadStage;

			if (MapManager.Instance.CurrentMapIdx != -1)
			{
				OnLoadStage(MapManager.Instance.CurrentStage);
			}
		}

		[SerializeField] private TextMeshProUGUI chapterText;
		[SerializeField] private TextMeshProUGUI stageText;
		[SerializeField] private TextMeshProUGUI cctvText;
		[SerializeField] private TextMeshProUGUI outputText;

		private void OnLoadStage(int idx)
		{
			chapterText.SetLocalizedString(Constants.TABLE_MENUUI, $"Chapter_{MapManager.Instance.CurrentChapter}");
			string localizedValue = string.Format(chapterText.GetComponent<LocalizeStringEvent>().StringReference.GetLocalizedString(), Constants.AREA_NUMBER[MapManager.Instance.CurrentChapter]);
			string finalString = $"{LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_MENUUI, "Chapter")}: {localizedValue}";
			chapterText.text = finalString;
			stageText.text = $"{LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_MENUUI, "Stage")}: {LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_MENUUI, $"Stage_{idx}")}";
			cctvText.text = $"CCTV: 00{MapManager.Instance.CurrentSaveIdx+1}";
			outputText.text = $"{LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_INOUT, "Output")}:\n {LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_INOUT, $"Output_{idx}")}";
		}
	}
}