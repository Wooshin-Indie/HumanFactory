using HumanFactory.Controller;

namespace HumanFactory.Buttons
{
	public class Jump0Button : TargetableButton, IJumpable
	{
		public override BuildingType ButtonType => BuildingType.Jump0;

		public bool IsAbleToJump(int humanNum)
		{
			if (buttonInfo.linkedGridPos.x < 0) return false;
			if (!isActive) return false;

			return humanNum == 0;
		}

		public override bool OnPressed(bool isToggled)
		{
			if (!base.OnPressed(isToggled)) return false;
			return true;
		}

		public override void OnButtonRightClick()
		{
			base.OnButtonRightClick();
			ToggleActive(false);
		}
	}
}
