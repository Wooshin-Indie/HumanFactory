using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseButtonEvent1 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color hoverColor = Color.grey;
    private TextMeshProUGUI buttonText;

    private void Start()
    {
        buttonText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
    }
}
