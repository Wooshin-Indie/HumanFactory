using UnityEngine;
using System.Collections.Generic;
using TMPro;
using HumanFactory.Manager;
using UnityEngine.UI;
using HumanFactory;

public class ResolutionDropdownUI : MonoBehaviour
{
	public TMP_Dropdown resolutionDropdown;
	private List<Resolution> ratioResolutions = new List<Resolution>();

	void Start()
	{
		
		for (int i = 0; i < Screen.resolutions.Length; i++)
		{
			float aspectRatio = (float)Screen.resolutions[i].width / Screen.resolutions[i].height;
			if (Mathf.Approximately(aspectRatio, 16f / 9f) &&
				!(ratioResolutions.Count != 0 && ratioResolutions[ratioResolutions.Count - 1].width == Screen.resolutions[i].width && ratioResolutions[ratioResolutions.Count - 1].height == Screen.resolutions[i].height))
			{
				ratioResolutions.Add(Screen.resolutions[i]);
			}
		}

		resolutionDropdown.ClearOptions();

		List<string> options = new List<string>();
		int currentResolutionIndex = 0;

		for (int i = 0; i < ratioResolutions.Count; i++)
		{
			string option = ratioResolutions[i].width + " x " + ratioResolutions[i].height;
			options.Add(option);

			if (ratioResolutions[i].width == Screen.currentResolution.width &&
				ratioResolutions[i].height == Screen.currentResolution.height)
			{
				currentResolutionIndex = i;
			}
		}

		resolutionDropdown.AddOptions(options);
		resolutionDropdown.value = currentResolutionIndex;
		resolutionDropdown.RefreshShownValue();

		resolutionDropdown.onValueChanged.AddListener(SetResolution);

		/** Toggles **/
		scanlineToggle.onValueChanged.AddListener(OnScanlineValueChanged);
		fullScreenToggle.onValueChanged.AddListener(OnFullScreenValueChanged);

		OnUpdateSetting(Managers.Data.BasicSettingData);
	}

	void SetResolution(int resolutionIndex)
	{
		Resolution resolution = ratioResolutions[resolutionIndex];
		Managers.Data.BasicSettingData.resolutionWidth = resolution.width;
		Managers.Data.BasicSettingData.resolutionHeight = resolution.height;
		Screen.SetResolution(resolution.width, resolution.height, Managers.Data.BasicSettingData.isFullScreen);
	}

	[SerializeField] private Toggle fullScreenToggle;
	[SerializeField] private Toggle scanlineToggle;

	private void OnUpdateSetting(SettingData data)
	{
		scanlineToggle.isOn = data.isScanline;
		fullScreenToggle.isOn = data.isFullScreen;
		for (int i = 0; i < ratioResolutions.Count; i++)
		{
			if (ratioResolutions[i].width == data.resolutionWidth && ratioResolutions[i].height == data.resolutionHeight)
			{
				resolutionDropdown.value = i;
				break;
			}
		}
	}
	private void OnScanlineValueChanged(bool isOn)
	{
		Managers.Data.BasicSettingData.isScanline = isOn;
		GameManagerEx.Instance.SetScanlineMaterial(isOn);
	}

	private void OnFullScreenValueChanged(bool isOn)
	{
		Managers.Data.BasicSettingData.isFullScreen = isOn;
		Screen.SetResolution(Managers.Data.BasicSettingData.resolutionWidth,
			Managers.Data.BasicSettingData.resolutionHeight,
			isOn);
	}
}