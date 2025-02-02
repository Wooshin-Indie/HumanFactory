using HumanFactory.Manager;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

namespace HumanFactory.Util.Effect
{
    public class SuccessPopupWritingEffect
    {
        private static AudioSource tmpSource = null;

        public static IEnumerator SuccessPopupWritingx(TextMeshProUGUI txtComponent, string key, float deltaTime)
		{
			string tmpDescript = LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, key);
			tmpSource?.Stop();
            tmpSource = Managers.Sound.PlaySfx(SFXType.Writing, 1.3f, 1.0f);
            txtComponent.text = "";

            foreach (char letter in tmpDescript.ToCharArray())
            {
                txtComponent.text += letter;

                float elapsedTime = 0f;
                while (elapsedTime > deltaTime)
                {
                    elapsedTime += Time.deltaTime;
                    yield return null; // 다음 프레임까지 대기
                }
            }

            txtComponent.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
            {
                TableReference = Constants.TABLE_SUCCESSPOPUPUI,
				TableEntryReference = key
            };

			tmpSource.Stop();
			tmpSource = null;
		}

    }
}