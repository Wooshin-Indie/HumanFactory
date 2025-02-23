
namespace HumanFactory.Buttons
{
	public class JumpButton : TargetableButton, IJumpable
	{
		public override BuildingType ButtonType => BuildingType.Jump;

		public bool IsAbleToJump(int humanNum)
		{
			if (buttonInfo.linkedGridPos.x < 0) return false;
			if (!isActive) return false;

			return true;
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
