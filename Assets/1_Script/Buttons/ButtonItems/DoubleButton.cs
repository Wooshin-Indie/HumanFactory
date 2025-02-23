using HumanFactory.Controller;
using HumanFactory.Manager;
using UnityEngine;

namespace HumanFactory.Buttons
{
	public class DoubleButton : NontargetableButton, IRotatable
	{
		public override BuildingType ButtonType => BuildingType.Double;

		public override bool OnPressed(bool isToggled)
		{
			if (!base.OnPressed(isToggled)) return false;
			return true;
		}


		public override bool DoButtonExecution(HumanController controller)
		{
			if (!base.DoButtonExecution(controller)) return false;
			PadType padType = MapManager.Instance.ProgramMap[buttonPos.x, buttonPos.y].PadType;

			if (padType == PadType.DirNone || buttonInfo.dirType == PadType.DirNone || (padType == buttonInfo.dirType))
			{
				controller.HumanNum *= 2;

				if (padType == PadType.DirNone && buttonInfo.dirType != PadType.DirNone)
					controller.UpdateTargetPosWithDoubleButtonDir(buttonInfo.dirType);
			}
			else
			{
				MapManager.Instance.AddPersonByDouble(controller, buttonInfo.dirType);
			}

			return true;
		}


		public override void OnButtonLeftClick(ref Vector2Int circuitingPos)
		{
			base.OnButtonLeftClick(ref circuitingPos);
			OnButtonRotate();
		}


		public void OnButtonRotate()
		{
			if (buttonInfo.dirType == PadType.DirNone)
			{
				buttonInfo.dirType = PadType.DirUp;
			}
			else
				buttonInfo.dirType = (PadType)((int)buttonInfo.dirType + 1);

			GetComponent<SpriteRenderer>().sprite = Managers.Resource.GetBuildingSprite(ButtonType, isPressed, isActive, buttonInfo.dirType);
		}
	}
}
