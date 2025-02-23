using HumanFactory;
using HumanFactory.Manager;
using HumanFactory.Util;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIOnClickExpand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private TextMeshProUGUI stageName;
    [SerializeField] private TextMeshProUGUI shortDesc;
    [SerializeField] private Sprite expandedSprite;

    private bool isExpanded = false;
    private Sprite originalSprite;
    private Vector2 originSize;
    private Vector2 expandedSize;
    private int id;

    private void Start()
    {
        originSize = GetComponent<RectTransform>().sizeDelta;
        expandedSize = new Vector2(originSize.x, originSize.y * 2f);
        originalSprite = GetComponent<Image>().sprite;
    }

    private Color GetStageColor(int id)
    {
        if (id < 0) return Color.white;

        if (!Managers.Data.IsAbleToAccessStage(id))
            return Constants.COLOR_LOCKSTAGE;
        if (Managers.Data.GetClientResultData(id).cycleCount < 0)
            return Constants.COLOR_UNLOCKSTAGE;

        return Constants.COLOR_CLEARSTAGE;
    }

    public void SetStageId(int id)
    {
        this.id = id;
        stageName.SetLocalizedString(Constants.TABLE_MENUUI, $"Stage_{id}");
        stageName.color = GetStageColor(id);
        shortDesc.SetLocalizedString(Constants.TABLE_STAGE, $"ShortDesc_{id}");
        
    }

    public void SetChapterId(int id)
	{
        this.id = -1;
		stageName.SetLocalizedString(Constants.TABLE_MENUUI, $"Chapter_{id}");
    }

    public void Expand()
    {
        isExpanded = true;

        GetComponent<RectTransform>().sizeDelta = expandedSize;

        GetComponent<Image>().sprite = expandedSprite;
    }

    public void Reduce()
    {
        isExpanded = false;

        GetComponent<RectTransform>().sizeDelta = originSize;

        GetComponent<Image>().sprite = originalSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Managers.Sound.PlaySfx(SFXType.UI_Hover);

        foreach(Transform child in transform)
        {
            if (child.GetComponent<TextMeshProUGUI>() != null)
                child.GetComponent<TextMeshProUGUI>().color = Color.black;
        }


        if (isExpanded)
        {
            GetComponent<Image>().sprite = originalSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<TextMeshProUGUI>() != null)
                child.GetComponent<TextMeshProUGUI>().color = GetStageColor(id);
        }

        if (isExpanded)
        {
            GetComponent<Image>().sprite = expandedSprite;
        }
    }
}