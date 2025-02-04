using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.Rendering;
using HumanFactory.Manager;
using HumanFactory.UI;

namespace HumanFactory
{
    public class CameraBase : MonoBehaviour
    {
        [Header("Effect")]
        [SerializeField] private GameObject volumeObject;   // GameScene에서만 필요
        [SerializeField] private CCTVUI cctvUI;             // GameScene에서만 필요

        [SerializeField] private CameraType backwardType;
        [SerializeField] private float duration;

        private bool isZoomed = false;
        private GameObject recentlyInteractedUI = null;

        public CCTVUI CctvUI { get => cctvUI; }
        public bool IsZoomed {  get { return isZoomed; } }

        private float originSize;
        private Vector3 originPos;

        private void Awake()
        {
            originSize = GetComponent<Camera>().orthographicSize;
            originPos = transform.localPosition;
        }

        /// <summary>
        /// 스크린을 클릭했을 때, 다른 카메라로 넘어가도록 하는 함수
        /// </summary>
        public void ConvertSceneForward(Vector2 destPos, float destSize, CameraType forwardType)
        {
            GameManagerEx.Instance.BlockAllUIs();
			if (GameManagerEx.Instance.CurrentCamType == CameraType.Game) return;

            // Position 움직이면서 Camera Size도 증가(ZoomIn)
            Vector3 dest = new Vector3(destPos.x, destPos.y, Constants.CAMERA_POS_Z);
            transform.DOLocalMove(dest, duration)
                .SetEase(Ease.Linear);
            GetComponent<Camera>().DOOrthoSize(destSize, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    GameManagerEx.Instance.Cameras[(int)forwardType].GetComponent<CameraBase>().StartVolumeOutCoroutine();
                    GameManagerEx.Instance.ChangeRenderCamera(forwardType);
                });
        }

        /// <summary>
        /// Object를 클릭했을 때, 해당 오브젝트로 ZoomIn 하는 함수
        /// </summary>
        public void ZoomInInteractable(Vector2 destPos, float destSize, GameObject interactUI)
        {

            Vector3 dest = new Vector3(destPos.x, destPos.y, Constants.CAMERA_POS_Z);

            recentlyInteractedUI = interactUI;
            interactUI?.SetActive(true);

            transform.DOLocalMove(dest, duration)
                .SetEase(Ease.Linear);
            GetComponent<Camera>().DOOrthoSize(destSize, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    isZoomed = true;
                    Managers.Input.ReleaseMouseInput();
                });
        }

        public void ConvertSceneBackward()
        {
            if (isZoomed)
            {
                recentlyInteractedUI?.GetComponent<PopUpUIBase>().CloseWindow();
                recentlyInteractedUI = null;
                isZoomed = false;
                ZoomOutToOrigin();
            }
            else
            {
                StartCoroutine(LerpToOriginCoroutine());
            }
        }





        // 다른 카메라에서 코루틴 실행하도록 하는 wrapper함수 
        private void StartVolumeOutCoroutine()
        {
            StartCoroutine(VolumeFadeOutCoroutine(duration));
        }

        private IEnumerator VolumeFadeOutCoroutine(float duration)
        {
            cctvUI?.LerpToAlpha0(duration);
            if (volumeObject == null)
            {
                Managers.Input.ReleaseMouseInput();
                yield break;
            }


            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                volumeObject.GetComponent<Volume>().weight = (1 - elapsedTime / duration);

                yield return null;
                elapsedTime += Time.deltaTime;
            }
            Managers.Input.ReleaseMouseInput();
        }

 
        private IEnumerator LerpToOriginCoroutine()
        {
            yield return StartCoroutine(VolumeFadeInCoroutine(duration));

            GameManagerEx.Instance.Cameras[(int)backwardType].GetComponent<CameraBase>().ZoomOutToOrigin();

            if (GameManagerEx.Instance.CurrentCamType != CameraType.Main)
                GameManagerEx.Instance.ChangeRenderCamera(backwardType);

        }

        private IEnumerator VolumeFadeInCoroutine(float duration)
        {
            cctvUI?.LerpToWhite(duration);
            if (volumeObject == null) yield break;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                volumeObject.GetComponent<Volume>().weight = (elapsedTime / duration);

                yield return null;
                elapsedTime += Time.deltaTime;
            }
        }

        private void ZoomOutToOrigin()
        {

            Vector3 dest = new Vector3(originPos.x, originPos.y, Constants.CAMERA_POS_Z);
            transform.DOLocalMove(dest, duration)
                .SetEase(Ease.Linear);

            GetComponent<Camera>().DOOrthoSize(originSize, duration)
                .SetEase(Ease.Linear).OnComplete(() =>
                {
                    Managers.Input.ReleaseMouseInput();
                });
        }

    }

}