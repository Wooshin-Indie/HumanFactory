using UnityEngine;
using UnityEngine.UIElements;

namespace HumanFactory.Props
{
    public class ClickableObject : ClickableBase
    {
        [SerializeField] private Vector3 relativeDestPos;
        [SerializeField] private GameObject interactUI;

        public override void OnPointerClick()
        {
            base.OnPointerClick();
            Vector3 pos = transform.localPosition;
            pos.z = Constants.CAMERA_POS_Z;
            Camera.main.GetComponent<CameraBase>().ZoomInInteractable(pos + relativeDestPos,
                zoomInCameraSize,
                interactUI);
        }
    }
}