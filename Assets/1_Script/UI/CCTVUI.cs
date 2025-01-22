using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
	public class CCTVUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI timeText;
		[SerializeField] private List<Image> cctvImage;

		[Header("Must Enable in GameScene")]
		[SerializeField] private InOutRevealUI inOutRevealUI;
		[SerializeField] private List<Transform> panels = new List<Transform>();

		public InOutRevealUI InOut { get => inOutRevealUI; }

		private int elapsedSeconds = 34567;

		private void Start()
		{
			StartCoroutine(TimerCoroutine());
		}

		private IEnumerator TimerCoroutine()
		{
			while (true)
			{
				timeText.text = FormatTime(elapsedSeconds++);
				yield return new WaitForSeconds(1f);
			}
		}

		private string FormatTime(int totalSeconds)
		{
			int hours = totalSeconds / 3600;
			int minutes = (totalSeconds % 3600) / 60;
			int seconds = totalSeconds % 60;

			return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
		}
		public void LerpToAlpha0(float duration)
		{
			for (int i = 0; i < cctvImage.Count; i++)
			{
				cctvImage[i].DOColor(Constants.COLOR_TRANS, duration)
					.SetEase(Ease.OutQuad);
			}
			timeText.DOColor(Constants.COLOR_TRANS, duration)
				.SetEase(Ease.OutQuad)
				.OnComplete(() =>
				{
					timeText.gameObject.SetActive(false);
					for (int i = 0; i < cctvImage.Count; i++)
					{
						cctvImage[i].gameObject.SetActive(false);
					}

					inOutRevealUI.gameObject.SetActive(true);

					for(int i=0; i<panels.Count; i++)
					{
						panels[i].gameObject.SetActive(true);	
					}
				});
		}

		public void LerpToWhite(float duration)
		{
			timeText.gameObject.SetActive(true);

			for (int i = 0; i < cctvImage.Count; i++)
			{
				cctvImage[i].gameObject.SetActive(true);
			}

			timeText.DOColor(Constants.COLOR_WHITE, duration)
				.SetEase(Ease.InQuad);
			for (int i = 0; i < cctvImage.Count; i++)
			{
				cctvImage[i].DOColor(Constants.COLOR_WHITE, duration)
					.SetEase(Ease.InQuad);
			}


			inOutRevealUI.gameObject.SetActive(false);

			for (int i = 0; i < panels.Count; i++)
			{
				panels[i].gameObject.SetActive(false);
			}
		}

	}
}