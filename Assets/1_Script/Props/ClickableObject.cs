using HumanFactory.Manager;
using UnityEngine;

namespace HumanFactory.Props
{
    public class ClickableObject : ClickableBase
    {
        [SerializeField] private Vector3 relativeDestPos;
        [SerializeField] private GameObject interactUI;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite hilightedSprite;


        private void Start()
        {
            GetComponent<SpriteRenderer>().sprite = defaultSprite;
        }

        public override void OnPointerClick()
        {
            base.OnPointerClick();
            Managers.Input.LockMouseInput();

            Vector3 pos = transform.localPosition;
            pos.z = Constants.CAMERA_POS_Z;
            Camera.main.GetComponent<CameraBase>().ZoomInInteractable(pos + relativeDestPos,
                zoomInCameraSize,
                interactUI);
        }

        public override void OnPointerEnter()
		{
			base.OnPointerEnter();
			GetComponent<SpriteRenderer>().sprite = hilightedSprite;
        }

        public override void OnPointerExit()
        {
            GetComponent<SpriteRenderer>().sprite = defaultSprite;
        }
    }
}