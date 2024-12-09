using UnityEngine;
using UnityEngine.EventSystems;

public class UIOnClickExpand : MonoBehaviour
{
    private Vector2 originSize;
    private Vector2 expandedSize;
    
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


    public void Expand()
    {
        GetComponent<RectTransform>().sizeDelta = expandedSize;
    }

    public void Reduce()
    {
        GetComponent<RectTransform>().sizeDelta = originSize;
    }
}