using UnityEngine;

namespace HumanFactory.Props
{
    public class ClickableBase : MonoBehaviour
    {
        [SerializeField] protected float zoomInCameraSize;
        [SerializeField] private GameObject outline;

        private void Start()
        {
            outline.SetActive(false);
        }

        public virtual void OnPointerEnter()
        {
            outline.SetActive(true);
        }

        public virtual void OnPointerExit()
        {
            outline.SetActive(false);
        }

        public virtual void OnPointerClick()
        {
            OnPointerExit();
        }


    }
}