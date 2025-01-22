using HumanFactory.Manager;
using UnityEngine;

namespace HumanFactory.Props
{
    public class ClickableScreen : ClickableBase
    {
        [SerializeField] private CameraType forwardType;

        public override void OnPointerClick()
        {


            base.OnPointerClick();

            Managers.Sound.PlaySfx(SFXType.Click);
            Camera.main.GetComponent<CameraBase>().ConvertSceneForward(transform.localPosition,
                zoomInCameraSize,
                forwardType);
        }
    }
}