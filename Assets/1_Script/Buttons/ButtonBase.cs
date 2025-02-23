using HumanFactory.Manager;
using UnityEngine;

namespace HumanFactory.Buttons
{
	[RequireComponent(typeof(SpriteRenderer))]
	public abstract class ButtonBase : MonoBehaviour
	{
		public Vector2Int buttonPos;

		public abstract BuildingType ButtonType { get; }
		public ButtonInfos buttonInfo;

		public void SetButtonBase(int x, int y, bool isActive)
		{
			buttonPos = new Vector2Int(x, y);
			GetComponent<SpriteRenderer>().sortingOrder = 5;
			transform.position = new Vector3(x, y, Constants.HUMAN_POS_Z);
			buttonInfo = new ButtonInfos(new Vector2Int(x, y));
			
			if (!isActive) ToggleActive(false);
		}

		public void SetButtonInfo(ButtonInfos buttonInfo)
		{
			this.buttonInfo = buttonInfo;
			var rotatble = this as IRotatable;

			if(rotatble != null)
			{
				GetComponent<SpriteRenderer>().sprite = Managers.Resource.GetBuildingSprite(ButtonType, false, isActive, buttonInfo.dirType);
			}
		}

		protected void Awake()
		{
			GetComponent<SpriteRenderer>().sprite = Managers.Resource.GetBuildingSprite(ButtonType, false, true);
		}

		private bool isInDragging = false;
		private Vector2 targetMousePos;
		/** Drag Events **/
		public void OnBeginDrag(Vector2 mousePos)
		{
		}
		public void OnDrag(Vector2 rowMousePos)
		{
			isInDragging = true;
			transform.position = new Vector3(rowMousePos.x, rowMousePos.y, Constants.HUMAN_POS_Z);
			transform.localScale = Vector3.one * .7f;
			SetSpriteColor(Constants.COLOR_INVISIBLE);
		}
		public void OnEndDrag(Vector2Int mousePos)
		{
			isInDragging = false;
			transform.position = new Vector3(mousePos.x, mousePos.y, Constants.HUMAN_POS_Z);
			transform.localScale = Vector3.one;
			buttonPos = new Vector2Int(mousePos.x, mousePos.y);
			SetSpriteColor(Constants.COLOR_WHITE);
		}

		public virtual void OnButtonRightClick()
		{
			Managers.Sound.PlaySfx(SFXType.UI_Hover, 1.0f, 0.8f);
		}

		public virtual void OnButtonLeftClick(ref Vector2Int circuitingPos)
		{
			Managers.Sound.PlaySfx(SFXType.UI_Hover, 1.0f, 0.8f);
		}

		protected bool isActive = true;
		protected bool isExecuted = false;
		protected bool isPressed = false;
		public bool IsActive { get => isActive; }
		public bool IsExecuted { get => isExecuted; }
		public bool IsPressed { get => isPressed; }

		public virtual bool OnPressed(bool isToggled)
		{
			Managers.Sound.PlaySfx(SFXType.ButtonPress);

			if (isExecuted) return false;

			if (isToggled)
			{
				isPressed = true;
				GetComponent<SpriteRenderer>().sprite = Managers.Resource.GetBuildingSprite(ButtonType, true, isActive, buttonInfo.dirType);
			}

			if (!isActive) return false;

			return true;
		}

		public virtual bool OnReleased(bool isToggled)
		{
			if (!isPressed) return false;
			if (isToggled)
			{
				isPressed = false;
				GetComponent<SpriteRenderer>().sprite = Managers.Resource.GetBuildingSprite(ButtonType, false, isActive, buttonInfo.dirType);
			}

			isExecuted = false;

			if (!isActive) return false;

			return true;
		}


		public void ToggleActive(bool isIngame)
		{
			if (ButtonType == BuildingType.Toggle) return;

			if (isIngame && isPressed)		// 눌려있는데 켜진 경우
			{
				if (isActive) OnReleased(false);
				else OnPressed(false);
			}
			isActive = !isActive;
			GetComponent<SpriteRenderer>().sprite = Managers.Resource.GetBuildingSprite(ButtonType, isPressed, isActive, buttonInfo.dirType);
		}

		/** Sprite Access **/
		public void SetSpriteColor(Color color)
		{
			GetComponent<SpriteRenderer>().color = color;
		}

	}
}