using UnityEngine;
using DG.Tweening;

namespace HumanFactory.Props
{
    public class ClickableScreenBase : MonoBehaviour
    {
        [SerializeField] protected Vector2 zoomInPosition;
        [SerializeField] protected float zoomInCameraSize;
        [SerializeField] private float zoomInDuration;
        [SerializeField] private GameObject outline;
        [SerializeField] private CameraType destination;

        private void Start()
        {
            outline.SetActive(false);
        }

        public void OnPointerEnter()
        {
            outline.SetActive(true);
        }

        public void OnPointerExit()
        {
            outline.SetActive(false);
        }

        public void OnPointerClick()
        {
            OnPointerExit();
            Camera.main.GetComponent<CameraBase>().LerpToScreen(zoomInPosition,
                zoomInCameraSize,
                zoomInDuration,
                AfterZoomIn);
        }

        private void AfterZoomIn()
        {
            GameManagerEx.Instance.ChangeRenderCamera(destination);
        }

    }
}