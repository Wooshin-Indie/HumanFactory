using HumanFactory.Buttons;
using Org.BouncyCastle.Math.Field;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace HumanFactory.Manager
{

	public class MapGrid
	{
		private int posX, posY;
		
		private SpriteRenderer arrowSprite;

		private PadType padType = PadType.DirNone;
		private PadType originPadType = PadType.DirNone;
		public PadType PadType { get => padType; set => padType = value; }
		public BuildingType ButtonType { get => (buttonBase == null ? BuildingType.None : buttonBase.ButtonType); }

		private ButtonBase buttonBase;
		public ButtonBase ButtonBase { get => buttonBase; set => buttonBase = value; }


		public MapGrid(int posX, int posY, SpriteRenderer arrow)
		{
			this.posX = posX;
			this.posY = posY;
			arrowSprite = arrow;
			padType = PadType.DirNone;
			originPadType = padType;
			arrow.color = Constants.COLOR_TRANS;

			arrow.sortingOrder = 4;
		}

		#region Pad Funcs
		public void GetPadParameter(out int dir)
		{
			dir = (int)padType;
		}

		public float GetPadAnimNormalizedTime()
		{
			return arrowSprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
		}

		public void OnClickRotate()
		{
			padType = (PadType)(((int)padType + 1) % Enum.GetValues(typeof(PadType)).Length);
			originPadType = padType;
			SetPad(padType);
		}

		private bool isSetPadByRot = false;

		public void SetPad(PadType type, bool isPermanent = true)
		{
			if (isPermanent)
			{
				padType = type;
				originPadType = padType;
			}
			else
			{
				if (isSetPadByRot)
				{
					if (padType != type) padType = PadType.DirNone;
				}
				else
				{
					padType = type;
				}
				isSetPadByRot = true;
			}

			switch (padType)
			{
				case PadType.DirLeft:
				case PadType.DirRight:
				case PadType.DirUp:
				case PadType.DirDown:
					arrowSprite.color = Constants.COLOR_ARROW;
					arrowSprite.transform.rotation = Quaternion.Euler(0f, 0f, -90f * (int)padType);
					break;
				case PadType.DirNone:
					arrowSprite.color = Constants.COLOR_TRANS;
					break;
			}
			AnimatorStateInfo state = arrowSprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
			arrowSprite.GetComponent<Animator>().Play(state.fullPathHash, 0, MapManager.Instance.GetElapsedAnimTime());
		}

		public void SetPadToOrigin()
		{
			SetPad(originPadType, true);
			isSetPadByRot = false;
		}
		#endregion

		public void SetButton(BuildingType type, bool isActive = true)
		{
			if (type == BuildingType.None)
			{
				EraseBuilding();
				return;
			}

			GameObject tmpGo = new GameObject { name = "AddButton" };
			switch (type)
			{
				case BuildingType.Add:
					buttonBase = tmpGo.AddComponent<AddButton>();
					break;
				case BuildingType.Sub:
					buttonBase = tmpGo.AddComponent<SubButton>();
					break;
				case BuildingType.Double:
					buttonBase = tmpGo.AddComponent<DoubleButton>();
					break;
				case BuildingType.Jump:
					buttonBase = tmpGo.AddComponent<JumpButton>();
					break;
				case BuildingType.Jump0:
					buttonBase = tmpGo.AddComponent<Jump0Button>();
					break;
				case BuildingType.Rotate:
					buttonBase = tmpGo.AddComponent<RotateButton>();
					break;
				case BuildingType.Toggle:
					buttonBase = tmpGo.AddComponent<ToggleButton>();
					break;
				case BuildingType.NewInput:
					buttonBase = tmpGo.AddComponent<InputButton>();
					break;
			}
			buttonBase.SetButtonBase(posX, posY, isActive);
			buttonBase.SetButtonInfo(new ButtonInfos(new Vector2Int(posX, posY)));
		}
		public void SetButton(StageGridData data)
		{
			if (data.buildingType == BuildingType.None)
			{
				EraseBuilding();
				return;
			}

			SetButton(data.buildingType, data.isActive);
			buttonBase.SetButtonInfo(new ButtonInfos(data.buttonInfos));
		}
		public void EraseBuilding()
		{
			if (buttonBase != null)
			{
				GameObject.Destroy(buttonBase.gameObject);
				buttonBase = null;
			}
		}

		public void OnReleased()
		{
			buttonBase?.OnReleased(true);
		}

		public void SetVisibility(InputMode mode)
		{

			switch (mode)
			{
				case InputMode.None:
					if (padType != PadType.DirNone) arrowSprite.color = Constants.COLOR_ARROW;
					buttonBase?.SetSpriteColor(Constants.COLOR_WHITE);
					break;
				case InputMode.Pad:
					if (padType != PadType.DirNone) arrowSprite.color = Constants.COLOR_ARROW;
					buttonBase?.SetSpriteColor(Constants.COLOR_INVISIBLE);
					break;
                case InputMode.Building:
					if (padType != PadType.DirNone) arrowSprite.color = Constants.COLOR_INVISIBLE;
					buttonBase?.SetSpriteColor(Constants.COLOR_WHITE);
					break;
			}
		}

		public void SetInvisibleInRemoveMode()
		{
            buttonBase?.SetSpriteColor(Constants.COLOR_INVISIBLE);
        }

        public void SetVisibleInRemoveMode()
        {
            buttonBase?.SetSpriteColor(Constants.COLOR_WHITE);
        }

        public StageGridData GetStageGridData()
		{
			StageGridData data = new StageGridData();
			data.posX = posX;
			data.posY = posY;
			data.padtype = padType;
			if (buttonBase == null)
			{
				data.buildingType = BuildingType.None;
			}
			else
			{
				data.buildingType = buttonBase.ButtonType;
				data.isActive = ButtonBase.IsActive;
				data.buttonInfos = new ButtonInfos(ButtonBase.buttonInfo);
			}

			return data;
		}

		public void SetStageGridInfo(StageGridData data)
		{
			posX = data.posX;
			posY = data.posY;
			SetPad(data.padtype);
			SetButton(data);
		}

		public void ClearGrid()
		{
			SetPad(PadType.DirNone);
			SetButton(BuildingType.None);
		}


	}
}