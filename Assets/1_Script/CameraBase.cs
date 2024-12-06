using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Rendering;
using System.Collections;

namespace HumanFactory
{
    public class CameraBase : MonoBehaviour
    {
        [Header("Effect")]
        [SerializeField] private GameObject volumeObject;
        [SerializeField] private float duration;
        [SerializeField] private CameraType forwardType;
        [SerializeField] private CameraType backwardType;

        [Header("DEBUG")]
        [SerializeField] private float originSize;
        [SerializeField] private Vector3 originPos;

        private void Awake()
        {
            originSize = GetComponent<Camera>().orthographicSize;
            originPos = transform.localPosition;
        }

        public void LerpToScreen(Vector2 destPos, float destSize)
        {
            if (GameManagerEx.Instance.CurrentCamType == CameraType.Game) return;

                Vector3 dest = new Vector3(destPos.x, destPos.y, Constants.CAMERA_POS_Z);
            transform.DOLocalMove(dest, duration)
                .SetEase(Ease.Linear);
            
            GetComponent<Camera>().DOOrthoSize(destSize, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    GameManagerEx.Instance.Cameras[(int)forwardType].GetComponent<CameraBase>().StartVolumeOutCoroutine();
                    GameManagerEx.Instance.ChangeRenderCamera(forwardType);
                    });
        }

        public void LerpToOrigin()
        {
            StartCoroutine(LerpToOriginCoroutine());
        }

        // 다른 카메라에서 코루틴 실행하도록 하는 wrapper함수 
        private void StartVolumeOutCoroutine()
        {
            StartCoroutine(VolumeFadeOutCoroutine(duration));
        }

        private IEnumerator VolumeFadeOutCoroutine(float duration)
        {
            if (volumeObject == null) yield break;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                volumeObject.GetComponent<Volume>().weight = (1 - elapsedTime / duration);

                yield return null;
                elapsedTime += Time.deltaTime;
            }
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
                .SetEase(Ease.Linear);
        }

    }

}