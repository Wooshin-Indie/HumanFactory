using HumanFactory.Manager;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class BuildingPanelItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemShortcut;

        [SerializeField] private Sprite selectedSprite;
        private Sprite originalSprite;
        private BuildingType myBuildingType = BuildingType.None;

        public void SetItemInfo(int index)
        {
            myBuildingType = (BuildingType)index;
            originalSprite = GetComponent<Image>().sprite;

            itemImage.sprite = Managers.Resource.GetBuildingSprite((BuildingType)index, false, true);
            itemName.text = Enum.GetName(typeof(BuildingType), index);
            itemShortcut.text = ((KeyCode)Managers.Data.BasicSettingData.KeyBindings[index]).ToString();
            SetItemName((BuildingType)index);

            GetComponent<Button>().onClick.AddListener(() =>
            {
                Managers.Input.ChangeCurSelectedBuilding((BuildingType)index);
            });

			Managers.Input.OnBuildingTypeChanged -= OnTypeChanged;
			Managers.Input.OnBuildingTypeChanged += OnTypeChanged;
        }

		private void OnDestroy()
        {
            Clear();
		}

        public void Clear()
		{
			Managers.Input.OnBuildingTypeChanged -= OnTypeChanged;
		}

		public void OnTypeChanged(BuildingType type)
        {
            if (type != myBuildingType)  // 다른애가 선택됨
            {
                if (isSelected)
                {
                    OnUnselected();
                }
            }
            else
            {
                if (!isSelected)
                {
                    OnSelected();
                }
            }
        }

        private bool isSelected = false;
        private void OnSelected()
		{
			isSelected = true;
            GetComponent<Image>().sprite = selectedSprite;
        }

        private void OnUnselected()
        {
            isSelected = false;
            GetComponent<Image>().sprite = originalSprite;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            itemName.GetComponent<TextMeshProUGUI>().color = Color.black;
            itemShortcut.GetComponent<TextMeshProUGUI>().color = Color.black;

			if (isSelected)
            {
                GetComponent<Image>().sprite = originalSprite;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {

            itemName.GetComponent<TextMeshProUGUI>().color = Color.white;
			itemShortcut.GetComponent<TextMeshProUGUI>().color = Color.white;

			if (isSelected)
            {
                GetComponent<Image>().sprite = selectedSprite;
            }
        }

        private void SetItemName(BuildingType type)
        {
            itemName.text = type.ToString();
        }
    }
}