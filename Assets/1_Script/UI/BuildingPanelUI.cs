using DG.Tweening;
using HumanFactory.Manager;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI {
    public class BuildingPanelUI : MonoBehaviour
    {
        
        [SerializeField] private RectTransform rect;
        [SerializeField] private float lerpDuration;
        [SerializeField] private Ease easeType;


        [Header("Content Setter")]
        [SerializeField] private Transform content;
        [SerializeField] private GameObject buildingItemPrefab;


        private Vector2 originalPos;
        private Vector2 hidePos;

        private string[] buildingTypes;

        private void Start()
        {
            originalPos = rect.anchoredPosition;
            hidePos = originalPos - new Vector2(- rect.rect.width * 1.5f, 0);

            rect.anchoredPosition = hidePos;

            Managers.Input.OnModeChangedAction -= OnModeChanged;
            Managers.Input.OnModeChangedAction += OnModeChanged;

            buildingTypes = Enum.GetNames(typeof(BuildingType));
            SetBanner();
        }

        private void OnModeChanged(InputMode mode)
        {
            if(mode == InputMode.Building)
            {
                ShowUI();
            }
            else
            {
                HideUI();
            }
        }

        private void HideUI()
        {
            rect.DOAnchorPos(hidePos, lerpDuration)
                .SetEase(easeType);
        }

        private void ShowUI()
        {
            rect.DOAnchorPos(originalPos, lerpDuration)
                .SetEase(easeType);
        }

        /// <summary>
        /// Start 에서 호출되어야 합니다.
        /// </summary>
        private void SetBanner()
        {
            for(int i =0; i<buildingTypes.Length-1; i++)
            {
                GameObject go = Instantiate(buildingItemPrefab, content);
                go.GetComponent<BuildingPanelItem>().SetItemInfo(i);
            }
        }
    }
}