using HumanFactory.Controller;
using HumanFactory.Manager;

namespace HumanFactory.Buttons
{ 
	public class InputButton : NontargetableButton
	{
		public override BuildingType ButtonType => BuildingType.NewInput;

		public override bool OnPressed(bool isToggled)
		{
			if (!base.OnPressed(isToggled)) return false;
			return true;
		}

		public override bool DoButtonExecution(HumanController controller)
		{
			if (!base.DoButtonExecution(controller)) return false;
			MapManager.Instance.AddPerson();
			return true;
		}
	}
}
