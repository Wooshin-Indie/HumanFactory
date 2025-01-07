using UnityEngine;

namespace HumanFactory.Props
{
    public class ClickableScreen : ClickableBase
    {
        [SerializeField] private CameraType forwardType;

        public override void OnPointerClick()
        {


            base.OnPointerClick();

            Camera.main.GetComponent<CameraBase>().ConvertSceneForward(transform.localPosition,
                zoomInCameraSize,
                forwardType);
        }
    }
}