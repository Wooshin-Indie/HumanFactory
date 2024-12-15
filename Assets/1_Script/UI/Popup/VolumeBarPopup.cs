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
            volumeSlider.onValueChanged.AddListener((value) =>
            {
                OnValueChanged(value);
            });
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
    }
}