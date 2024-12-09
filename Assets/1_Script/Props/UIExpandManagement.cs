using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIExpandManagement : MonoBehaviour, IPointerClickHandler
{
    List<GameObject> stages = new List<GameObject>();
    [SerializeField] private GameObject stagePanel;

    private void Start()
    {
        for (int i = 0; i < 20; i++)
        {

            stages.Add(Instantiate(stagePanel, this.transform));
        }
        

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        


    }
}