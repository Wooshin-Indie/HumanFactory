using HumanFactory.Manager;
using HumanFactory.Util;
using TMPro;
using UnityEngine;
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
		[SerializeField] private TextMeshProUGUI inputText;
		[SerializeField] private TextMeshProUGUI outputText;

		private void OnLoadStage(int idx)
		{
			chapterText.text = $"{LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_MENUUI, "Chapter")}: {LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_MENUUI, $"Chapter_{MapManager.Instance.CurrentChapter}")}";
			stageText.text = $"{LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_MENUUI, "Stage")}: {LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_MENUUI, $"Stage_{idx}")}";
			cctvText.text = $"CCTV: 00{MapManager.Instance.CurrentSaveIdx+1}";
			inputText.text = $"{LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_INOUT, "Input")}: {LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_INOUT, $"Input_{idx}")}";
			outputText.text = $"{LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_INOUT, "Output")}: {LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_INOUT, $"Output_{idx}")}";
		}
	}
}