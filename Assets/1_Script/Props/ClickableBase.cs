using UnityEngine;

namespace HumanFactory.Props
{
    public class ClickableBase : MonoBehaviour
    {
        [SerializeField] protected float zoomInCameraSize;
        [SerializeField] protected GameObject outline;

        public virtual void OnPointerEnter() { }

        public virtual void OnPointerExit() { }

        public virtual void OnPointerClick()
        {
            OnPointerExit();
        }
    }
}