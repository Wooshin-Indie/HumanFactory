using HumanFactory.Manager;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class ExeTypeUI : MonoBehaviour
    {

		[SerializeField] private Image exeTypeImage;
		[SerializeField] private TextMeshProUGUI exeTypeText;

		[SerializeField] private List<Sprite> sprites = new List<Sprite>();

		public enum ExeSpriteType
		{
			None,
			Play,
			Pause,
			Fast,
		}

		private void Start()
		{
			GameManagerEx.Instance.OnExeTypeChange -= OnExeTypeChanged;
			GameManagerEx.Instance.OnExeTypeChange += OnExeTypeChanged;

			MapManager.Instance.OnCycleTimeChanged -= OnCycleTimeChanged;
			MapManager.Instance.OnCycleTimeChanged += OnCycleTimeChanged;
		}

		private void OnEnable()
		{
			OnExeTypeChanged(ExecuteType.None);
		}

		private void OnCycleTimeChanged(float cycleTime)
		{
			if (GameManagerEx.Instance.ExeType == ExecuteType.Play)
			{
				if (Mathf.Approximately(cycleTime, 1f))
					exeTypeImage.sprite = sprites[(int)ExeSpriteType.Play];
				else
					exeTypeImage.sprite = sprites[(int)ExeSpriteType.Fast];
				exeTypeText.text = $"{(int)(Mathf.Round(1/cycleTime))}x";
			}
		}

		private void OnExeTypeChanged(ExecuteType type)
		{
			switch (type)
			{
				case ExecuteType.None:
					exeTypeImage.sprite = sprites[(int)ExeSpriteType.None];
					exeTypeText.text = $"";
					break;
				case ExecuteType.Play:
					if (MapManager.Instance.CycleTime < 1f)
						exeTypeImage.sprite = sprites[(int)ExeSpriteType.Fast];
					else
						exeTypeImage.sprite = sprites[(int)ExeSpriteType.Play];
					exeTypeText.text = $"{(int)(Mathf.Round(1/MapManager.Instance.CycleTime))}x";
					break;
				case ExecuteType.Pause:
					exeTypeImage.sprite = sprites[(int)ExeSpriteType.Pause];
					exeTypeText.text = $"Pause";
					break;
			}
		}
	}
}