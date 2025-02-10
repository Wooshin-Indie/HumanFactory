using HumanFactory.Manager;
using UnityEngine;

namespace HumanFactory.Props
{
    public class ClickableBase : MonoBehaviour
    {
        [SerializeField] protected float zoomInCameraSize;
        [SerializeField] protected GameObject outline;

        public virtual void OnPointerEnter()
		{
			Managers.Sound.PlaySfx(SFXType.UI_Hover);
		}

        public virtual void OnPointerExit() { }

        public virtual void OnPointerClick()
		{
			Managers.Sound.PlaySfx(SFXType.Wind, 0.6f, 1.1f);
			OnPointerExit();
        }
    }
}