using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIExpandManagement : MonoBehaviour
{
    List<GameObject> stages = new List<GameObject>();
    [SerializeField] private GameObject stagePanel;
    int currentExpandedPanel = -1;

    private void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            int c = i;

            stages.Add(Instantiate(stagePanel, this.transform));
            stages[c].GetComponent<Button>().onClick.AddListener(() =>
            {
                OnClickStages(c);
            });
        }
        

    }

    public void OnClickStages(int index)
    {
        if (currentExpandedPanel == -1) // 아무것도 확대 안돼있음
        {
            stages[index].GetComponent<UIOnClickExpand>().Expand();
            currentExpandedPanel = index;
        }
        else if (currentExpandedPanel == index) // 이미 확대돼있던걸 클릭 -> 축소
        {
            stages[index].GetComponent<UIOnClickExpand>().Reduce();
            currentExpandedPanel = -1;
        }
        else if (currentExpandedPanel != index) // 확대 돼있는게 있는 상태에서 다른 패널 클릭
        {
            stages[currentExpandedPanel].GetComponent<UIOnClickExpand>().Reduce();
            stages[index].GetComponent<UIOnClickExpand>().Expand();
            currentExpandedPanel = index;
        }
    }
}