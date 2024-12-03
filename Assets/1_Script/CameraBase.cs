using UnityEngine;
using DG.Tweening;
using System;

namespace HumanFactory
{
    public class CameraBase : MonoBehaviour
    {
        [SerializeField] private float originSize;
        [SerializeField] private Vector3 originPos;

        private void Awake()
        {
            originSize = GetComponent<Camera>().orthographicSize;
            originPos = transform.position;
        }

        public void LerpToScreen(Vector2 destPos, float destSize, float duration, Action complete)
        {
            Vector3 dest = new Vector3(destPos.x, destPos.y, Constants.CAMERA_POS_Z);
            transform.DOLocalMove(dest, duration)
                .SetEase(Ease.OutQuad);
            
            GetComponent<Camera>().DOOrthoSize(destSize, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    complete.Invoke();
                    });
        }

        public void LerpToOrigin()
        {

        }
    }

}