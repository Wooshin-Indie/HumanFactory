using UnityEngine;
using System.Collections.Generic;
using TMPro;

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
	}

	void SetResolution(int resolutionIndex)
	{
		Resolution resolution = ratioResolutions[resolutionIndex];
		Screen.SetResolution(resolution.width, resolution.height, 
			resolution.width == 1920 && resolution.width==0);
	}
}