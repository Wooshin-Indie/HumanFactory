using HumanFactory.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class CarssettePopup : PopUpUIBase
    {
        [SerializeField] private PopUpUIBase volumeSetUI;
        [SerializeField] private TextMeshProUGUI musicNameText;

        // Editor 상에서 Button에 연결
        public void OnButonClick(bool isNext)
        {
            Managers.Sound.ChangeBGM(isNext);
            UpdateBGMText();
        }

        // OnEnable, Disable 시 추가적으로 필요한 작업 진행
        public override void OnEnable()
        {
            base.OnEnable();
            volumeSetUI.gameObject.SetActive(true);
            UpdateBGMText();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void PopupWindow()
        {
            base.PopupWindow();
        }

        public override void CloseWindow()
        {
            base.CloseWindow();
            volumeSetUI.CloseWindow();
        }

        private int currentSelectedIdx = 0;


        private void UpdateBGMText()
        {
            AudioClip clip = Managers.Resource.GetBGM((BGMType)Managers.Sound.CurrentBGM);
            musicNameText.text = (clip == null) ? "None" : clip.name;
        }
    }
}