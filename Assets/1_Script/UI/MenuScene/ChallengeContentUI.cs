using HumanFactory.Manager;
using HumanFactory.Util;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class ChallengeContentUI : MonoBehaviour
    {
        [SerializeField] List<TextMeshProUGUI> itemNameTexts = new List<TextMeshProUGUI>();
        [SerializeField] List<TextMeshProUGUI> itemScoreTexts = new List<TextMeshProUGUI>();
		[SerializeField] List<TextMeshProUGUI> graphNameTexts = new List<TextMeshProUGUI>();
		[SerializeField] List<Image> scoreGraphs = new List<Image>();
        [SerializeField] List<Image> maxGraphs = new List<Image>();

        public void ClearInfo()
        {
            gameObject.SetActive(false);
        }

        public void SetStageInfo(int index)
		{
			this.gameObject.SetActive(true);
            StageResultData resultData = Managers.Data.GamePlayData.stageGridDatas[index].resultDatas;
            StageInfo info = Managers.Resource.GetStageInfo(index);
            int[] counts = {
                resultData.CycleCount,
                resultData.ButtonCount,
                resultData.KillCount
            };

            for (int i = 0; i < itemScoreTexts.Count; i++)
			{
				itemScoreTexts[i].text = $"{counts[i]}/{info.challenges[i]}";
			}

            float maxHeight = GetComponent<RectTransform>().rect.height;
            float originWidth = maxGraphs[0].GetComponent<RectTransform>().sizeDelta.x;
        
            itemNameTexts[0].SetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Challenge_Cycle");
            itemNameTexts[1].SetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Challenge_Structures");
            itemNameTexts[2].SetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Challenge_Died");
			graphNameTexts[0].SetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Challenge_Cycle");
			graphNameTexts[1].SetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Challenge_Structures");
			graphNameTexts[2].SetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Challenge_Died");

            for (int i = 0; i < maxGraphs.Count; i++)
            {
                if (counts[i] == -1)
				{
                    GrowGraph(i, 0f, maxHeight * 0.6f);
					scoreGraphs[i].color = Constants.COLOR_RED;
					scoreGraphs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Constants.COLOR_RED;
				}
                else if (counts[i] <= info.challenges[i])
				{
                    GrowGraph(i, maxHeight * 0.6f * counts[i] / info.challenges[i], maxHeight * 0.6f);
                    scoreGraphs[i].color = Constants.COLOR_GREEN;
                    scoreGraphs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Constants.COLOR_GREEN;
				}
                else
				{
                    GrowGraph(i, maxHeight * 0.6f, maxHeight * 0.6f * info.challenges[i] / counts[i]);
					scoreGraphs[i].color = Constants.COLOR_RED;
					scoreGraphs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Constants.COLOR_RED;
				}
				maxGraphs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = info.challenges[i].ToString();
				scoreGraphs[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = counts[i].ToString();
			}
        }

        private bool[] isGrowing = new bool[3] { false, false, false };
        private Coroutine[] graphCoroutine = new Coroutine[3] { null, null, null };
        private float maxDuration = .6f;

        private void GrowGraph(int index, float scoreHeight, float maxHeight)
        {
            if (isGrowing[index]) StopCoroutine(graphCoroutine[index]);
            graphCoroutine[index] = StartCoroutine(GrowGraphCoroutine(index, scoreHeight, maxHeight));
        }

        private IEnumerator GrowGraphCoroutine(int index, float scoreHeight, float maxHeight)
        {
            isGrowing[index] = true;
            float tmpMax = Mathf.Max(scoreHeight, maxHeight);
            float elapsedTime = 0f;
			float originWidth = maxGraphs[0].GetComponent<RectTransform>().sizeDelta.x;

            scoreGraphs[index].GetComponent<RectTransform>().sizeDelta = new Vector2(originWidth, 0);
            maxGraphs[index].GetComponent<RectTransform>().sizeDelta = new Vector2(originWidth, 0);

            yield return new WaitForSeconds(0.1f * index);
            while (elapsedTime < maxDuration)
            {
                if (scoreGraphs[index].GetComponent<RectTransform>().sizeDelta.y < scoreHeight)
                    scoreGraphs[index].GetComponent<RectTransform>().sizeDelta
                        = new Vector2(originWidth, scoreGraphs[index].GetComponent<RectTransform>().sizeDelta.y + tmpMax * Time.deltaTime / maxDuration);
				if (maxGraphs[index].GetComponent<RectTransform>().sizeDelta.y < maxHeight)
					maxGraphs[index].GetComponent<RectTransform>().sizeDelta
						= new Vector2(originWidth, maxGraphs[index].GetComponent<RectTransform>().sizeDelta.y + tmpMax * Time.deltaTime / maxDuration);

				elapsedTime += Time.deltaTime;
                yield return null;
            }

            isGrowing[index] = false;
        }

    }
}