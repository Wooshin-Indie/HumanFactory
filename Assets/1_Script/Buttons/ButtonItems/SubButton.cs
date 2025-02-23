using HumanFactory.Controller;

namespace HumanFactory.Buttons
{
	public class SubButton : NontargetableButton
	{
		public override BuildingType ButtonType => BuildingType.Sub;

		public override bool OnPressed(bool isToggled)
		{
			if (base.OnPressed(isToggled)) return false;
			return true;
		}

		public override bool DoButtonExecution(HumanController controller)
		{
			if (!base.DoButtonExecution(controller)) return false;
			controller.SubByButton();
			return true;
		}

	}
}
