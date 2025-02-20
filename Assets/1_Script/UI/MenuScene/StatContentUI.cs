using HumanFactory.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace HumanFactory.UI
{
    public class StatContentUI : MonoBehaviour
    {
        [SerializeField] private List<StatContentItem> items = new List<StatContentItem>();

        public void ClearInfo()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].gameObject.SetActive(false);
            }
        }

        public void SetStageInfo(int stageIdx)
		{

            if (!Managers.Data.IsAbleToAccessStage(stageIdx))   // 접근불가
			{
				for (int i = 0; i < items.Count; i++)
				{
					items[i].gameObject.SetActive(false);
				}
			}
            else
			{
				CountResultData serverData = Managers.Data.GetServerResults(stageIdx);
				StageResultData clientData = Managers.Data.GetClientResultData(stageIdx);

				for (int i = 0; i < items.Count; i++)
				{
					items[i].gameObject.SetActive(true);
				}

				// Set Graphs
				items[0].SetGraph(stageIdx, 0, serverData.cycleGraphs, clientData.cycleCount);
				items[1].SetGraph(stageIdx, 1, serverData.buttonGraphs, clientData.buttonCount);
				items[2].SetGraph(stageIdx, 2, serverData.killGraphs, clientData.killCount);
			}
		}
    }
}