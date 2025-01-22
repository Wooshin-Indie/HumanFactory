using HumanFactory.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HumanFactory.UI
{
	public class UIItemBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private TextMeshProUGUI stageName;
		[SerializeField] private Sprite selectedSprite;

		private bool isSelected = false;
		private Sprite originalSprite;

		private void Awake()
		{
			originalSprite = GetComponent<Image>().sprite;
		}

		public void OnSelected(bool isSelected)
		{
			this.isSelected = isSelected;
			if (isSelected)
			{
				GetComponent<Image>().sprite = selectedSprite;
			}
			else
			{
				GetComponent<Image>().sprite = originalSprite;
			}
		}

		private bool isHighlighting = false;
		public void OnHighlight()
		{
			isHighlighting = true;

			foreach (Transform child in transform)
			{
				if (child.GetComponent<TextMeshProUGUI>() != null)
					child.GetComponent<TextMeshProUGUI>().color = Color.black;
			}

			GetComponent<Image>().sprite = GetComponent<Button>().spriteState.highlightedSprite;
		}

		public void OffHighLight()
		{
			isHighlighting = false;
			foreach (Transform child in transform)
			{
				if (child.GetComponent<TextMeshProUGUI>() != null)
					child.GetComponent<TextMeshProUGUI>().color = Color.white;
			}

			GetComponent<Image>().sprite = originalSprite;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			Managers.Sound.PlaySfx(SFXType.UI_Hover);
			foreach (Transform child in transform)
			{
				if (child.GetComponent<TextMeshProUGUI>() != null)
					child.GetComponent<TextMeshProUGUI>().color = Color.black;
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (isHighlighting) return;
			foreach (Transform child in transform)
			{
				if (child.GetComponent<TextMeshProUGUI>() != null)
					child.GetComponent<TextMeshProUGUI>().color = Color.white;
			}

			if (isSelected)
			{
				GetComponent<Image>().sprite = selectedSprite;
			}
		}
	}
}