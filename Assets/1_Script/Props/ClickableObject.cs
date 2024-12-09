using UnityEngine;

namespace HumanFactory.Props
{
    public class ClickableObject : ClickableBase
    {
        [SerializeField] private Vector3 destPos;
        [SerializeField] private GameObject interactUI;

        public override void OnPointerClick()
        {
            base.OnPointerClick();
            Camera.main.GetComponent<CameraBase>().ZoomInInteractable(destPos,
                zoomInCameraSize,
                interactUI);
        }
    }
}