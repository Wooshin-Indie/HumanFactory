using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HumanFactory.UI
{
    public class LogPanelUI : MonoBehaviour
    {
        [SerializeField] private Transform logParent;
        [SerializeField] private GameObject logPrefab;
        [SerializeField] private float logDuration;

        Queue<Transform> logQueue = new Queue<Transform>();

        public void DisplayLog(string message, Color? color = null)
        {
            GameObject tmpLog = Instantiate(logPrefab, logParent);
            tmpLog.GetComponent<TextMeshProUGUI>().text = message;
            tmpLog.GetComponent<TextMeshProUGUI>().color = color ?? Color.white;

            tmpLog.GetComponent<TextMeshProUGUI>().DOFade(0f, logDuration).SetDelay(2f);
            logQueue.Enqueue(tmpLog.transform);
            Invoke(nameof(OnTimeout), logDuration);
        }

        private void OnTimeout()
		{
			if (logQueue.Count != 0)
			{
				Destroy(logQueue.Dequeue().gameObject);
			}
		}

        private void ClearQueue()
        {
            while (logQueue.Count != 0)
            {
                Destroy(logQueue.Dequeue().gameObject);
            }
        }
    }
}