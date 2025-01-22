using HumanFactory.Manager;
using System.Collections;
using UnityEngine;

namespace HumanFactory.Effects
{
    public class TVNoiseEffect : MonoBehaviour
    {
        [SerializeField] private Material renderMat;
        [SerializeField] private Material noiseMat;
        [SerializeField] private float noiseTime;

        private bool isTempNoising = false;
        private bool isNoising = false;
        private Coroutine noiseCoroutine = null;

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
            if (isTempNoising) {
                elapsedTime = 0f;
                return;
            }

			isTempNoising = true;
            SetNoiseMat();
			noiseCoroutine = StartCoroutine(NoiseCoroutine());
        }

        public void SetPermanentNoise()
		{
			if (isTempNoising)
            {
                StopCoroutine(noiseCoroutine);
				isTempNoising = false;
			}
            SetNoiseMat();
		}

		private IEnumerator NoiseCoroutine()
		{
			Managers.Sound.PlaySfx(SFXType.Noise, 0.3f);
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
			isTempNoising = false; 
            SetRenderMat();
		}


        private void SetNoiseMat()
        {
            if (isNoising) return;

			GetComponent<MeshRenderer>().material = noiseMat;
            Managers.Input.LockMenuInput();
            isNoising = true;
		}
		private void SetRenderMat()
		{
			if (!isNoising) return;

			GetComponent<MeshRenderer>().material = renderMat;
			Managers.Input.ReleaseMenuInput();
			isNoising = false;
		}
	}

}