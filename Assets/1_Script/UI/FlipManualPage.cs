using DG.Tweening;
using HumanFactory.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class FlipManualPage : MonoBehaviour
    {

		[SerializeField] private Button nextPageButton;
		[SerializeField] private Button prevPageButton;
		[SerializeField] private float moveDistance;
		[SerializeField] private float moveDuration;

        private List<RectTransform> pages = new List<RectTransform>();
		private int curPage = 0;

		private bool isPaging = false;

		private void Awake()
		{
			// 시작하자마자 자식에 있는 (정렬된)페이지들을 pages에 저장

			foreach (Transform child in transform)
			{
				RectTransform rectTransform = child.GetComponent<RectTransform>();
				if (rectTransform != null && child.CompareTag(Constants.TAG_PAGES))
				{
					pages.Add(rectTransform);
				}
			}

			pages = pages.OrderBy(item => item.name).ToList();

			ResetChildOrder();

			nextPageButton.onClick.AddListener(OnNextPage);
			prevPageButton.onClick.AddListener(OnPrevPage);
		}

		private void OnEnable()
		{
			Managers.Sound.PlaySfx(SFXType.UI_Paper);
			curPage = 0;
			ResetChildOrder();
			isPaging = false;

			if (curPage == 0)
			{
				prevPageButton.gameObject.SetActive(false);
			}
		}

		private void OnNextPage()
		{ 
			if (curPage == pages.Count() - 1) return;
			
			if (isPaging) return;
			isPaging = true;

			Managers.Sound.PlaySfx(SFXType.UI_Paper);

			int nextPage = curPage + 1;

			if(nextPage == pages.Count() - 1)
			{
				nextPageButton.gameObject.SetActive(false);
			}
			if (nextPage == 1)
			{
				prevPageButton.gameObject.SetActive(true);
			}

			Sequence seq1 = DOTween.Sequence();
			Sequence seq2 = DOTween.Sequence();

			pages[nextPage].gameObject.SetActive(true);
			seq1.Append(pages[nextPage].DOAnchorPosX(moveDistance, moveDuration));
			seq1.Join(pages[nextPage].DOLocalRotate(new Vector3(0, 0, -20f), moveDuration));
			seq1.Append(pages[nextPage].DOAnchorPosX(0, moveDuration));
			seq1.Join(pages[nextPage].DOLocalRotate(new Vector3(0, 0, 0f), moveDuration));

			seq2.Append(pages[curPage].DOAnchorPosX(-moveDistance, moveDuration)
				.OnComplete(() => {
					pages[nextPage].SetSiblingIndex(pages.Count() - 1);
				}));
			seq2.Join(pages[curPage].DOLocalRotate(new Vector3(0, 0, 20), moveDuration));
			seq2.Append(pages[curPage].DOAnchorPosX(0, moveDuration)
				.OnComplete(() =>
				{
					pages[curPage].gameObject.SetActive(false);
					curPage++;
					isPaging = false;
				}));
			seq2.Join(pages[curPage].DOLocalRotate(new Vector3(0, 0, 0), moveDuration));

			seq1.Play();
			seq2.Play();
		}

		private void OnPrevPage()
		{
			if (curPage == 0) return;

			if (isPaging) return;
			isPaging = true;

			Managers.Sound.PlaySfx(SFXType.UI_Paper);
			int prevPage = curPage - 1;

			if (prevPage == pages.Count() - 2)
			{
				nextPageButton.gameObject.SetActive(true);
			}
			if (prevPage == 0)
			{
				prevPageButton.gameObject.SetActive(false);
			}

			Sequence seq1 = DOTween.Sequence();
			Sequence seq2 = DOTween.Sequence();

			pages[prevPage].gameObject.SetActive(true);
			seq1.Append(pages[prevPage].DOAnchorPosX(-moveDistance, moveDuration));
			seq1.Join(pages[prevPage].DOLocalRotate(new Vector3(0, 0, 20f), moveDuration));
			seq1.Append(pages[prevPage].DOAnchorPosX(0, moveDuration));
			seq1.Join(pages[prevPage].DOLocalRotate(new Vector3(0, 0, 0), moveDuration));

			seq2.Append(pages[curPage].DOAnchorPosX(moveDistance, moveDuration)
				.OnComplete(() => {
					pages[prevPage].SetSiblingIndex(pages.Count()-1);
				}));
			seq2.Join(pages[curPage].DOLocalRotate(new Vector3(0, 0, -20), moveDuration));
			seq2.Append(pages[curPage].DOAnchorPosX(0, moveDuration)
				.OnComplete(() =>
				{
					pages[curPage].gameObject.SetActive(false);
					curPage--;
					isPaging = false;
				}));
			seq2.Join(pages[curPage].DOLocalRotate(new Vector3(0, 0, 0), moveDuration));
			
			seq1.Play();
			seq2.Play();


		}

		private void ResetChildOrder()
		{
			for (int i = pages.Count()-1; i >=0; i--)
			{
				pages[i].SetSiblingIndex(pages.Count()-i-1);
				pages[i].gameObject.SetActive(false);
			}
			pages[curPage].gameObject.SetActive(true);
		}
	}
}