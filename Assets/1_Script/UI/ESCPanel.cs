using HumanFactory.Manager;
using HumanFactory.Props;
using Org.BouncyCastle.Asn1.Crmf;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class ESCPanel : PopUpUIBase
    {
		[SerializeField] private List<Button> buttons = new List<Button>();
        [SerializeField] private Button optionButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Image pointImage;
		[SerializeField] private ClickableScreen settingScreen;

		public override void Awake()
		{
			base.Awake();
			buttons[0].onClick.AddListener(OnContinueButton);
			buttons[1].onClick.AddListener(OnOptionButton);
			buttons[2].onClick.AddListener(OnExitButton);

			for (int i = 0; i < buttons.Count; i++)
			{
				int t = i;

				EventTrigger.Entry enterTrigger = new EventTrigger.Entry
				{
					eventID = EventTriggerType.PointerEnter
				};
				enterTrigger.callback.AddListener((data) => { OnPointerItem(t); });
				buttons[i].GetComponent<EventTrigger>().triggers.Add(enterTrigger);

				EventTrigger.Entry exitTrigger = new EventTrigger.Entry
				{
					eventID = EventTriggerType.PointerExit
				};
				exitTrigger.callback.AddListener((data) => { OnPointerExitItem(t); });
				buttons[i].GetComponent<EventTrigger>().triggers.Add(exitTrigger);
			}
		}

		private void OnPointerItem(int idx)
		{
			pointImage.gameObject.SetActive(true);
			pointImage.rectTransform.anchoredPosition = buttons[idx].GetComponent<RectTransform>().anchoredPosition
				+ new Vector2(-buttons[idx].GetComponent<RectTransform>().rect.width/2-50f, 0);
			buttons[idx].GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);
		}

		private void OnPointerExitItem(int idx)
		{
			pointImage.gameObject.SetActive(false);
			buttons[idx].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		}


		private void OnContinueButton()
		{
			Camera.main.GetComponent<CameraBase>().UnlockPanel();
		}

		private void OnOptionButton()
		{
			Camera.main.GetComponent<CameraBase>().UnlockPanel();
			settingScreen.OnPointerClick();
		}

		public override void CloseWindow()
		{
			base.CloseWindow();
			pointImage.gameObject.SetActive(false);
			for(int i=0; i<buttons.Count; i++)
			{
				buttons[i].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			}
		}

		private void OnExitButton()
		{
            Managers.Data.SaveAll();
            Application.Quit();
		}

	}
}