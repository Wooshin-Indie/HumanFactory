using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
	public class DescScrollHelper : MonoBehaviour
	{
		[SerializeField] private ScrollRect scroll;
		[SerializeField] private RectTransform content;
		[SerializeField] private Button button;

		private void Start()
		{
			scroll.onValueChanged.AddListener(OnScrollValueChanged);

			UpdateButtonVisibility();


		}

		private void OnScrollValueChanged(Vector2 scrollPosition)
		{
			UpdateButtonVisibility();
		}

		private void UpdateButtonVisibility()
		{
			// Viewport 높이
			RectTransform viewport = scroll.viewport;
			float viewportHeight = viewport.rect.height;

			// 스크롤 위치 계산
			float contentHeight = content.rect.height;
			float contentBottomPosition = content.anchoredPosition.y + viewportHeight;

			if (contentHeight > contentBottomPosition + 5)
			{
				button.gameObject.SetActive(true);
			}
			else
			{
				button.gameObject.SetActive(false);
			}
		}

		private IEnumerator ScrollDown()
		{
			float duration = 0.5f; 
			float elapsedTime = 0f;

			float startValue = scroll.verticalNormalizedPosition;
			float targetValue = 0f;

			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				float t = elapsedTime / duration;

				scroll.verticalNormalizedPosition = Mathf.Lerp(startValue, targetValue, t);
				yield return null;
			}

			scroll.verticalNormalizedPosition = targetValue;
		}
	}
}