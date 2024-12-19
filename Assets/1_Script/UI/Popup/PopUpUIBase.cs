using DG.Tweening;
using UnityEngine;

namespace HumanFactory.UI
{
    public class PopUpUIBase : MonoBehaviour
    {
        private RectTransform rect;

        public virtual void Awake()
        {
            rect = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }
        public virtual void Start()
        {
        }

        [Header("Popup Info")]
        [SerializeField] private Vector2 outScreenPos;
        [SerializeField] private Vector2 inScreenPos;
        [SerializeField] private float duration;
        [SerializeField] private Ease easeType;

        /** virtual functions **/

        /** 외부에서 호출될 함수들 **/
        public virtual void PopupWindow()
        {
            rect.DOAnchorPos(inScreenPos, duration)
                .SetEase(easeType);
        }

        public virtual void CloseWindow()
        {
            rect.DOAnchorPos(outScreenPos, duration)
                .SetEase(easeType)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
        }

        public virtual void OnEnable()
        {
            PopupWindow();
        }

        public virtual void OnDisable()
        {
            // 끌 때 outScreenPos로 옮겨둬야함
            rect.anchoredPosition = outScreenPos;
        }

    }
}