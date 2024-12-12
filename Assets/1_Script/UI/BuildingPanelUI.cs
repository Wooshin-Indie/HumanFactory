using DG.Tweening;
using HumanFactory.Manager;
using UnityEngine;

namespace HumanFactory.UI {
    public class BuildingPanelUI : MonoBehaviour
    {
        
        [SerializeField] private RectTransform rect;
        [SerializeField] private float lerpDuration;
        [SerializeField] private Ease easeType;

        private Vector2 originalPos;
        private Vector2 hidePos;

        private void Start()
        {
            originalPos = rect.anchoredPosition;
            hidePos = originalPos - new Vector2(- rect.rect.width * 1.5f, 0);

            rect.anchoredPosition = hidePos;

            Managers.Input.OnModeChangedAction -= OnModeChanged;
            Managers.Input.OnModeChangedAction += OnModeChanged;
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

    }
}