using HumanFactory;
using HumanFactory.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOnClickExpand : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI stageName;
    
    private Vector2 originSize;
    private Vector2 expandedSize;

    private void Start()
    {
        originSize = GetComponent<RectTransform>().sizeDelta;
        expandedSize = new Vector2(originSize.x, originSize.y * 2f);
    }

    public void SetStageId(int id)
    {
        stageName.text = Managers.Resource.GetStageInfo(id).stageName;
    }

    public void Expand()
    {
        GetComponent<RectTransform>().sizeDelta = expandedSize;
    }

    public void Reduce()
    {
        GetComponent<RectTransform>().sizeDelta = originSize;
    }
}