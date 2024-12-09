using DG.Tweening;
using UnityEngine;

namespace HumanFactory.UI
{
    public class PopUpUIBase : MonoBehaviour
    {
        private RectTransform rect;
        private void Awake()
        {
            rect = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        [SerializeField] private Vector2 outScreenPos;
        [SerializeField] private Vector2 inScreenPos;
        [SerializeField] private float duration;
        [SerializeField] private Ease easeType;

        /** 외부에서 호출될 함수들 **/
        public void PopupWindow()
        {
            rect.DOAnchorPos(inScreenPos, duration)
                .SetEase(easeType);
        }

        public void CloseWindow()
        {
            rect.DOAnchorPos(outScreenPos, duration)
                .SetEase(easeType)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }

        public void OnEnable()
        {
            PopupWindow();
        }

        public void OnDisable()
        {
            // 끌 때 outScreenPos로 옮겨둬야함
            rect.anchoredPosition = outScreenPos;
        }

    }
}