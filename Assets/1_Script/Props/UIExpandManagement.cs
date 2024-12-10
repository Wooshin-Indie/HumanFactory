using DG.Tweening;
using HumanFactory.Effects;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIExpandManagement : MonoBehaviour
{
    private List<GameObject> stages = new List<GameObject>();

    [SerializeField] private GameObject stagePanelPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private float lerpDuration;
    [SerializeField] private TVNoiseEffect noiseEffect;


    private Tweener scrollTweener;
    private int currentExpandedPanel = -1;
    private float scrollviewHeight;


    private void Start()
    {
        for (int i = 0; i < 20; i++)
        {
            int idx = i;
            stages.Add(Instantiate(stagePanelPrefab, content));
            stages[idx].GetComponent<UIOnClickExpand>().SetStageId(idx);
            stages[idx].GetComponent<Button>().onClick.AddListener(() =>
            {
                OnClickStages(idx);
            });
        }

        scrollviewHeight = GetComponent<RectTransform>().rect.height;
        content.GetComponent<VerticalLayoutGroup>().padding.bottom = (int)scrollviewHeight / 2;
        content.GetComponent<VerticalLayoutGroup>().padding.top = (int)scrollviewHeight / 2;
    }

    private void OnClickStages(int index)
    {
        if (currentExpandedPanel == -1) // 아무것도 확대 안돼있음
        {
            stages[index].GetComponent<UIOnClickExpand>().Expand();
            SetSelectedStageOnCenter(index, lerpDuration);
            currentExpandedPanel = index;
        }
        else if (currentExpandedPanel == index) // 이미 확대돼있던걸 클릭 -> 축소
        {
            stages[index].GetComponent<UIOnClickExpand>().Reduce();
            currentExpandedPanel = -1;
            scrollTweener.Kill();
        }
        else if (currentExpandedPanel != index) // 확대 돼있는게 있는 상태에서 다른 패널 클릭
        {
            stages[currentExpandedPanel].GetComponent<UIOnClickExpand>().Reduce();
            stages[index].GetComponent<UIOnClickExpand>().Expand();
            SetSelectedStageOnCenter(index, lerpDuration);
            currentExpandedPanel = index;
        }
    }

    // 선택된(확장된) 스테이지가 가운데 오도록 합니다.
    // Content 의 AnchoredPosition.y를 움직이며, (0, content.height - scollview.height) 범위를 갖습니다.
    // Expand 한 후에 호출되기 때문에, 각 Item의 height 의 절반만큼 조정해줍니다
    private void SetSelectedStageOnCenter(int index, float duration)
    {
        noiseEffect.MakeNoise();
        float maxPosition = content.GetComponent<RectTransform>().rect.height - scrollviewHeight + 200;
        float setPosition = Mathf.Clamp(Mathf.Abs(stages[index].GetComponent<RectTransform>().anchoredPosition.y) - scrollviewHeight / 2,
            0,
            maxPosition);

        scrollTweener = content.GetComponent<RectTransform>().DOAnchorPosY(setPosition, duration);
    }
}