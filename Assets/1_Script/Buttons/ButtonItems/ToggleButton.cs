using HumanFactory.Manager;

namespace HumanFactory.Buttons
{
	public class ToggleButton : TargetableButton
	{
		public override BuildingType ButtonType => BuildingType.Toggle;

		public override bool OnPressed(bool isToggled)
		{
			if (!base.OnPressed(isToggled)) return false;
			MapManager.Instance.ToggleButtonInGame(buttonInfo.linkedGridPos.x, buttonInfo.linkedGridPos.y);
			return true;
		}

		public override bool OnReleased(bool isToggled)
		{
			if (!base.OnReleased(isToggled)) return false;
			MapManager.Instance.ToggleButtonInGame(buttonInfo.linkedGridPos.x,
				buttonInfo.linkedGridPos.y);

			return true;
		}

		public override void OnButtonRightClick()
		{
			base.OnButtonRightClick();
		}
	}
}
