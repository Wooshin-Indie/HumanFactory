using HumanFactory.Manager;
using UnityEngine;

namespace HumanFactory.Props
{
    public class ClickableObjectLetter : ClickableBase
    {
        [SerializeField] private Vector3 relativeDestPos;
        [SerializeField] private GameObject interactUI;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite hilightedSprite;
        [SerializeField] private float hilightDuration;


        private void Start()
        {
            GetComponent<SpriteRenderer>().sprite = defaultSprite;
            prevSprite = defaultSprite;
        }

        private float elapsedTime = 0;
        Sprite prevSprite;
        private void Update()
        {
            if (Managers.Data.GamePlayData.isLetterOpened || isEntered) return;

            elapsedTime += Time.deltaTime;

            if (elapsedTime > hilightDuration)
            {
                prevSprite = (prevSprite == hilightedSprite ? defaultSprite : hilightedSprite);
                GetComponent<SpriteRenderer>().sprite = prevSprite;
                elapsedTime = 0;
            }
        }

        public override void OnPointerClick()
        {
            base.OnPointerClick();
            Managers.Input.LockMouseInput();

            Managers.Data.GamePlayData.isLetterOpened = true;
            Managers.Data.SaveGameplayData();

            Vector3 pos = transform.localPosition;
            pos.z = Constants.CAMERA_POS_Z;
            Camera.main.GetComponent<CameraBase>().ZoomInInteractable(pos + relativeDestPos,
                zoomInCameraSize,
                interactUI);
        }

        private bool isEntered = false;
        public override void OnPointerEnter()
		{
			base.OnPointerEnter();
			isEntered = true;
            GetComponent<SpriteRenderer>().sprite = hilightedSprite;
        }

        public override void OnPointerExit()
        {
            isEntered = false;
            GetComponent<SpriteRenderer>().sprite = defaultSprite;
            elapsedTime = 0;
        }
    }
}