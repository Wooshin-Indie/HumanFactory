using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace HumanFactory.UI
{
	public class DescScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] ScrollRect scroll;

		private bool isHolding = false;
		private Coroutine scrollCoroutine;


		public void OnPointerDown(PointerEventData eventData)
		{
			if (scrollCoroutine != null)
				StopCoroutine(scrollCoroutine);

			isHolding = true;
			scrollCoroutine = StartCoroutine(ScrollDownContinuously());
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			isHolding = false;

			if (scrollCoroutine != null)
			{
				StopCoroutine(scrollCoroutine);
				scrollCoroutine = null;
			}
		}

		private IEnumerator ScrollDownContinuously()
		{
			while (isHolding)
			{
				scroll.verticalNormalizedPosition = Mathf.Max(scroll.verticalNormalizedPosition - 0.2f, 0f);
				yield return new WaitForSeconds(Time.deltaTime * 50); // 스크롤 속도 조절
			}
		}
	}
}