using System;
using System.Data;
using UnityEngine;

namespace HumanFactory.Manager
{

	public class MapGrid
	{
		private int posX, posY;
		private SpriteRenderer arrowSprite;
		private SpriteRenderer buildingSprite;
		private PadType padType = PadType.DirNone;
		private BuildingType buildingType = BuildingType.None;
		private ButtonInfos buttonInfo = new ButtonInfos(new Vector2Int(-1, -1));

		public int PosX { get => posX; }
		public int PosY { get => posY; }
		public PadType PadType { get => padType; set => padType = value; }
		public BuildingType BuildingType { get => buildingType; set => buildingType = value; }
		public ButtonInfos ButtonInfo { get => buttonInfo; set => buttonInfo = value; }

		public Sprite BuildingSprite { get => buildingSprite.sprite; }

		private bool isPressed = false;
		public bool IsPressed { get => isPressed; set => isPressed = value; }

		private bool isActive = true;
		public bool IsActive { get => isActive; set => isActive = value; }


		private PadType originPadType = 0;

		public MapGrid(int posX, int posY, SpriteRenderer arrow, SpriteRenderer building)
		{
			this.posX = posX;
			this.posY = posY;
			arrowSprite = arrow;
			buildingSprite = building;
			padType = PadType.DirNone;
			originPadType = padType;
			arrow.color = Constants.COLOR_TRANS;
			building.sprite = null;
			building.color = Color.white;
			buttonInfo.linkedGridPos.Set(posX, posY);
		}

		public void GetPadParameter(out int dir)
		{
			dir = (int)padType;
		}

		public void OnClickRotate()
		{
			padType = (PadType)(((int)padType + 1) % Enum.GetValues(typeof(PadType)).Length);
			originPadType = padType;
			SetPad(padType);
		}

		public void SetPad(PadType type, bool isPermanent = true)
		{
			padType = type;
			if (isPermanent) originPadType = padType;
			switch (type)
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
		}

		public void SetPadToOrigin()
		{
			SetPad(originPadType);
		}


		public void SetBuilding(BuildingType type)
		{
			buildingType = type;
			if (type != BuildingType.None)
			{
				isActive = true;
				buildingSprite.sprite = Managers.Resource.GetBuildingSprite(type, false, isActive);
				buildingSprite.color = Color.white;
			}
			else
				buildingSprite.sprite = null;
		}

		public void PreviewBuilding(BuildingType type)
		{
			if (type != BuildingType.None)
			{
				buildingSprite.sprite = Managers.Resource.GetBuildingSprite(type, false, true);
				buildingSprite.color = Constants.COLOR_INVISIBLE;
			}
		}

		public void UnpreviewBuilding()
		{
			if (buildingType != BuildingType.None) return;

			buildingSprite.sprite = null;
		}

		public void OnRelease()
		{
			if (!isPressed) return;
			isPressed = false;
			buildingSprite.sprite = Managers.Resource.GetBuildingSprite(buildingType, false, isActive, buttonInfo.dirType);


			if (!isActive) return;
			switch (buildingType)
			{
				case BuildingType.Button:
					break;
				case BuildingType.ToggleButton:
					MapManager.Instance.ToggleButtonInGame(buttonInfo.linkedGridPos.x,
						buttonInfo.linkedGridPos.y);
					break;
				case BuildingType.RotateButton:
					MapManager.Instance.PadToOrigin(buttonInfo.linkedGridPos.x,
						buttonInfo.linkedGridPos.y);
					break;
			}
		}

		public void OnPressed()
		{
			if (isPressed) return;
			isPressed = true;
			buildingSprite.sprite = Managers.Resource.GetBuildingSprite(buildingType, true, isActive, buttonInfo.dirType);

			Debug.Log("PRESSED");
			if (!isActive) return;
			switch (buildingType)
			{
				case BuildingType.Button:
					MapManager.Instance.AddPerson();
					break;
				case BuildingType.ToggleButton:
					MapManager.Instance.ToggleButtonInGame(buttonInfo.linkedGridPos.x,
						buttonInfo.linkedGridPos.y);
					break;
				case BuildingType.RotateButton:
					Debug.Log("ROTATE : " + buttonInfo.linkedGridPos.ToString() + ", " + buttonInfo.dirType);
					MapManager.Instance.RotatePadDir(buttonInfo.linkedGridPos.x,
						buttonInfo.linkedGridPos.y,
						buttonInfo.dirType);
					break;
			}
		}

		public void SetVisibility(InputMode mode)
		{

			switch (mode)
			{
				case InputMode.None:
					if (padType != PadType.DirNone) arrowSprite.color = Constants.COLOR_ARROW;
					buildingSprite.color = Constants.COLOR_WHITE;
					break;
				case InputMode.Pad:
					if (padType != PadType.DirNone) arrowSprite.color = Constants.COLOR_ARROW;
					buildingSprite.color = Constants.COLOR_INVISIBLE;
					break;
				case InputMode.Building:
					if (padType != PadType.DirNone) arrowSprite.color = Constants.COLOR_INVISIBLE;
					buildingSprite.color = Constants.COLOR_WHITE;
					break;
			}
		}

		public StageGridData GetStageGridData()
		{
			StageGridData data = new StageGridData();
			data.posX = posX;
			data.posY = posY;
			data.padtype = padType;
			data.buildingType = buildingType;
			data.isActive = isActive;
			data.buttonInfos = new ButtonInfos(buttonInfo);

			return data;
		}

		public void SetStageGridInfo(StageGridData data)
		{
			posX = data.posX;
			posY = data.posY;
			SetPad(data.padtype);
			SetBuilding(data.buildingType);
			if (!data.isActive)
			{
				Debug.Log("?? : " + PosX + ", " + PosY);
				ToggleActive(false);
			}
			buttonInfo = new ButtonInfos(data.buttonInfos);
			buildingSprite.sprite = Managers.Resource.GetBuildingSprite(BuildingType, isPressed, isActive, buttonInfo.dirType);
	
		}

		public void ClearGrid()
		{
			SetPad(PadType.DirNone);
			SetBuilding(BuildingType.None);
			buttonInfo = new ButtonInfos(new Vector2Int(posX, posY));
		}

		public void ToggleActive(bool isIngame)
		{
			if (buildingType == BuildingType.None || buildingType == BuildingType.ToggleButton) return;

			if (isIngame && isPressed)
			{
				if (isActive) OnRelease();
				else OnPressed();
			}
			isActive = !isActive;
			buildingSprite.sprite = Managers.Resource.GetBuildingSprite(buildingType, isPressed, isActive, buttonInfo.dirType);
		}

		public void OnButtonRotate()
		{
			if (buttonInfo.dirType == PadType.DirNone)
			{
				buttonInfo.dirType = PadType.DirUp;
			}
			else
				buttonInfo.dirType = (PadType)((int)buttonInfo.dirType + 1);

			buildingSprite.sprite = Managers.Resource.GetBuildingSprite(BuildingType, isPressed, isActive, buttonInfo.dirType);
		}
	}
}