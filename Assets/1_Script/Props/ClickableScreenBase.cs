using UnityEngine;

namespace HumanFactory.Props
{
    public class ClickableScreenBase : MonoBehaviour
    {
        [SerializeField] protected float zoomInCameraSize;
        [SerializeField] private GameObject outline;

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
            Camera.main.GetComponent<CameraBase>().LerpToScreen(transform.localPosition,
                zoomInCameraSize);
        }


    }
}