using HumanFactory.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.Props
{
    public class ClickableScreen : ClickableBase
    {
        [SerializeField] private CameraType forwardType;

        private bool isBlocked = false;
        public bool IsBlocked { get => isBlocked; }
        private void Start()
        {
            outline.SetActive(false);
        }

        public override void OnPointerClick()
		{
			if (isBlocked) return;

			Managers.Input.LockMouseInput();
			base.OnPointerClick();

            Managers.Sound.PlaySfx(SFXType.Click);
            Camera.main.GetComponent<CameraBase>().ConvertSceneForward(transform.localPosition,
                zoomInCameraSize,
                forwardType);
        }

		public override void OnPointerEnter()
		{
            if (isBlocked)
			{
				OnPointerExit();
                return;
            }

			base.OnPointerEnter();
			outline.SetActive(true);
		}

		public override void OnPointerExit()
		{
            if (isBlocked) return;

            outline.SetActive(false);
        }

		public void BlockClick(bool isBlock)
        {
            isBlocked = isBlock;
		}

	}
}