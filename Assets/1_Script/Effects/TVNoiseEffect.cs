using System.Collections;
using UnityEngine;

namespace HumanFactory.Effects
{
    public class TVNoiseEffect : MonoBehaviour
    {
        [SerializeField] private Material renderMat;
        [SerializeField] private Material noiseMat;
        [SerializeField] private float noiseTime;

        private bool isNoising = false;

        private void Awake()
        {
            renderMat = GetComponent<MeshRenderer>().material;
        }

        /// <summary>
        /// time 동안 noise를 발생시킵니다.
        /// </summary>
        float elapsedTime = 0f;
        float maxTime;
        public void MakeNoise()
        {
            maxTime = noiseTime;
            if (isNoising) {
                elapsedTime = 0f;
                return;
            }

            isNoising = true;
            GetComponent<MeshRenderer>().material = noiseMat;
            StartCoroutine(NoiseCoroutine());
        }

        private IEnumerator NoiseCoroutine()
        {
            elapsedTime = 0f;
            while (elapsedTime < maxTime)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            OnReleaseNoise();
        }

        private void OnReleaseNoise()
        {
            isNoising = false;
            GetComponent<MeshRenderer>().material = renderMat;
        }
    }

}