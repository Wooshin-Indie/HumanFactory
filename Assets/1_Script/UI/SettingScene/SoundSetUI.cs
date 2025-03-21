using HumanFactory.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class SoundSetUI : MonoBehaviour
    {
        [SerializeField] private Slider masterSlider;
		[SerializeField] private Slider bgmSlider;
		[SerializeField] private Slider sfxSlider;

		private void Start()
		{
			masterSlider.onValueChanged.AddListener((value) =>
			{
				Managers.Sound.MasterVolume = value;
				Managers.Data.UpdateBasicSettingChanges();
			});
			sfxSlider.onValueChanged.AddListener((value) =>
			{
				Managers.Sound.SfxVolume = value;
				Managers.Data.UpdateBasicSettingChanges();
			});
			bgmSlider.onValueChanged.AddListener((value) =>
			{
				Managers.Sound.BgmVolume = value;
				Managers.Data.UpdateBasicSettingChanges();
			});

			Managers.Data.OnUpdateBasicSettings -= OnValueChangedFromOuter;
			Managers.Data.OnUpdateBasicSettings += OnValueChangedFromOuter;

			OnValueChangedFromOuter(Managers.Data.BasicSettingData);
		}

		private void OnValueChangedFromOuter(SettingData data)
		{
			masterSlider.value = data.masterVolume;
			bgmSlider.value = data.bgmVolume;
			sfxSlider.value = data.sfxVolume;
		}
	}
}