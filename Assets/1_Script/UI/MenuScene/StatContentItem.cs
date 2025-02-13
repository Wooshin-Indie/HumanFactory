using HumanFactory.Manager;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
	public class StatContentItem : MonoBehaviour
	{
		[SerializeField] private GameObject barPrefab;
		[SerializeField] private List<Image> bars = new List<Image>();
		[SerializeField] private TextMeshProUGUI maxValueText;
		[SerializeField] private RectTransform resultPointer;
		[SerializeField] private Image barBase;

		public void Start()
		{
			for (int i = 0; i < 20; i++)
			{
				bars.Add(Instantiate(barPrefab, barBase.transform).GetComponent<Image>());
			}
		}

		public void SetGraph(int stageIdx, int graphIdx, int[] graphs, int value)
		{
			if (graphs == null || graphs.Length == 0) return;

			int maxResult = graphs.Max();
			float width = barBase.rectTransform.rect.width / 20f;
			float maxHeight = 100f;

			for (int i = 0; i < bars.Count; i++)
			{
				bars[i].rectTransform.sizeDelta = new Vector2(width, maxHeight * graphs[i] / maxResult);
				bars[i].rectTransform.anchoredPosition = new Vector3(width * i, 0, 0);
			}

			int maxValue = Managers.Resource.GetStageInfo(stageIdx).maxCounts[graphIdx];
			maxValueText.text = maxValue.ToString();

			int clientIdx = Mathf.Min((value * Constants.COUNT_GRAPH_MAX / maxValue), Constants.COUNT_GRAPH_MAX - 1);
			if (value < 0)
			{
				resultPointer.gameObject.SetActive(false);
			}
			else
			{
				resultPointer.gameObject.SetActive(true);
				resultPointer.anchoredPosition = new Vector3((clientIdx + 0.5f) * width, 120f, 0f);
				resultPointer.GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
			}
		}
	}
}