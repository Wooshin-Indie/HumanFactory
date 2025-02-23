using HumanFactory.Controller;
using HumanFactory.Manager;
using Org.BouncyCastle.Crypto.Prng;
using UnityEngine;

namespace HumanFactory.Buttons
{
	public class RotateButton : TargetableButton, IRotatable
	{
		public override BuildingType ButtonType => BuildingType.Rotate;

		public override bool OnPressed(bool isToggled)
		{
			if (!base.OnPressed(isToggled)) return false;
			MapManager.Instance.RotatePadDir(buttonInfo.linkedGridPos.x, buttonInfo.linkedGridPos.y, buttonInfo.dirType);
			return true;
		}

		public override bool OnReleased(bool isToggled)
		{
			if (!base.OnReleased(isToggled)) return false;
			MapManager.Instance.PadToOrigin(buttonInfo.linkedGridPos.x,
				buttonInfo.linkedGridPos.y);
			return true;
		}

		public override void OnButtonRightClick()
		{
			base.OnButtonRightClick();
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

			GetComponent<SpriteRenderer>().sprite = Managers.Resource.GetBuildingSprite(ButtonType, isPressed, true, buttonInfo.dirType);
		}
	}
}
