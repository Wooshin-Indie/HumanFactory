using HumanFactory.Manager;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

namespace HumanFactory.Util.Effect
{
    public static class TypingEffect
    {
        private static AudioSource tmpSource = null;
        public static IEnumerator TypingCoroutine(TextMeshProUGUI tmp, string key, float deltaTime)
		{
			string tmpDescript = LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_STAGE, key);
			tmpSource?.Stop();
            tmpSource = Managers.Sound.PlaySfx(SFXType.Typing, 1.3f, 1.0f);
            tmp.text = "";

            foreach (char letter in tmpDescript.ToCharArray())
            {
                tmp.text += letter;
                yield return new WaitForSeconds(deltaTime);
            }

            tmp.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
            {
                TableReference = Constants.TABLE_STAGE,
				TableEntryReference = key
            };

			tmpSource.Stop();
			tmpSource = null;
		}

	}
}