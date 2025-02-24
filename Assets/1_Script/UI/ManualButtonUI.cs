using DG.Tweening;
using HumanFactory.Manager;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class ManualButtonUI : MonoBehaviour
		, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button button;
        [SerializeField] private GameObject manualPopup;

		private void Awake()
		{
			button.onClick.AddListener(() => OnManualButtonClick());
		}

		private void OnManualButtonClick()
		{
			if (!Managers.Input.IsMouseInputEnabled()) return;

			Managers.Input.LockMouseInput();

			Vector3 pos = Camera.main.transform.position;
			pos.z = Constants.CAMERA_POS_Z;
			Camera.main.GetComponent<CameraBase>().ZoomInInteractable(pos,
				 Camera.main.orthographicSize,
				manualPopup);

			GetComponent<RectTransform>().DOAnchorPosY(0f, 0.3f);
		}

		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
		{
			if (!Managers.Input.IsMouseInputEnabled()) return;
			GetComponent<RectTransform>().DOAnchorPosY(30f, 0.3f);
		}

		void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
		{
			if (!Managers.Input.IsMouseInputEnabled()) return;
			GetComponent<RectTransform>().DOAnchorPosY(0f, 0.3f);
		}
	}
}