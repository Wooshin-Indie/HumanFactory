using HumanFactory.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class VolumeBarPopup : PopUpUIBase
    {
        [SerializeField] private Slider volumeSlider;

        public override void Awake()
        {
            base.Awake();
        }

		public override void Start()
		{
            base.Start();
			volumeSlider.onValueChanged.AddListener((value) =>
			{
				OnValueChanged(value);
				Managers.Data.UpdateBasicSettingChanges();
			});

			Managers.Data.OnUpdateBasicSettings -= OnValueChangeFromOuter;
			Managers.Data.OnUpdateBasicSettings += OnValueChangeFromOuter;
		}

		public override void PopupWindow()
        {
            volumeSlider.value = Managers.Data.BasicSettingData.BgmVolume;
            base.PopupWindow();
        }

        // Slider의 값이 바뀔 때 호출되는 함수입니다.
        private void OnValueChanged(float value)
        {
            Managers.Sound.BgmVolume = value;
        }

        private void OnValueChangeFromOuter(SettingData data)
        {
            volumeSlider.value = data.BgmVolume;
        }
    }
}