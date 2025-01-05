using DG.Tweening;
using HumanFactory.Effects;
using HumanFactory.Manager;
using HumanFactory.Util.Effect;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIExpandManagement : MonoBehaviour
{
    private List<GameObject> stages = new List<GameObject>();

    [Header("Interact Effects")]
    [SerializeField] private GameObject stagePanelPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private float lerpDuration;

    [Header("Additional Subjects")]
    [SerializeField] private TVNoiseEffect noiseEffect;
    [SerializeField] private TextMeshProUGUI stageDescript;


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
        if (typeCoroutine != null) StopCoroutine(typeCoroutine);
        stageDescript.text = "";

        if (currentExpandedPanel == -1) // 아무것도 확대 안돼있음
        {
            stages[index].GetComponent<UIOnClickExpand>().Expand();
            SetSelectedStageOnCenter(index, lerpDuration);
            currentExpandedPanel = index;
            MapManager.Instance.LoadStage(currentExpandedPanel);
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
			MapManager.Instance.LoadStage(currentExpandedPanel);
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


        StartTypingDescript(index);
    }

    private Coroutine typeCoroutine = null;
    private void StartTypingDescript(int index)
    {
        stageDescript.GetComponent<RectTransform>().DOAnchorPosY(0f, 0f);
        // 이건 나중에 ResourceManager나 Localization에서 받아옴
        string tmpDescript = "Stage #1 - Mov\r\n\r\nThis is your first task. \r\nIt's very simple, but you must not take it lightly. \r\nThe people you see on the screen will obey your commands without question. \r\nIf you tell them to go, they will go... and if you tell them to die, they will die\r\n";
        typeCoroutine = StartCoroutine(TypingEffect.TypingCoroutine(stageDescript, tmpDescript, 0.01f));
    }
}