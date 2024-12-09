using UnityEngine;
using UnityEngine.EventSystems;

public class UIOnClickExpand : MonoBehaviour, IPointerClickHandler
{
    private Vector2 originSize;
    private Vector2 expandedSize;
    private bool isExpanded = false;

    public int index = 0;
    
    private void Start()
    {
        
        originSize = GetComponent<RectTransform>().sizeDelta;
        expandedSize = new Vector2(originSize.x, originSize.y * 2f);
    }
    /*
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = bigScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originScale;
    }
    */

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(Input.mousePosition);
        if (isExpanded)
        {
            GetComponent<RectTransform>().sizeDelta = originSize;
        }
        else
        {
            GetComponent<RectTransform>().sizeDelta = expandedSize;
        }
        isExpanded = !isExpanded;
    }
}