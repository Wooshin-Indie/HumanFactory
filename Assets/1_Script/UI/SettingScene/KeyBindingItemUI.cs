using HumanFactory.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace HumanFactory.UI
{
    public class KeyBindingItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI actionText;
        [SerializeField] private TextMeshProUGUI keyText;
        [SerializeField] private Button bindingButton;

        private ShortcutActionEnum actionEnum;

		public void Start()
		{
            bindingButton.onClick.AddListener(() =>
            {
                OnStartBinding();
				Managers.Input.StartBinding(actionEnum, OnEndBinding);
            });
		}

		public void OnUpdateBinding(ShortcutActionEnum keyEnum)
        {
            actionEnum = keyEnum;
            actionText.text = keyEnum.ToString();
            int keyCode = Managers.Data.BasicSettingData.KeyBindings[(int)keyEnum];
            if (keyCode == (int)KeyCode.None) keyText.text = "";
            else keyText.text = ((KeyCode)keyCode).ToString();
        }

        private void OnStartBinding()
        {
            bindingButton.GetComponent<UIItemBase>().OnHighlight();
        }

        public void OnEndBinding()
        {
            bindingButton.GetComponent<UIItemBase>().OffHighLight();
        }
    }
}