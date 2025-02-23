using HumanFactory.Controller;
using UnityEngine;

namespace HumanFactory.Buttons
{
	public abstract class NontargetableButton : ButtonBase
	{
		public override void OnButtonRightClick()
		{
			base.OnButtonRightClick();
			ToggleActive(false);
		}

		public virtual bool DoButtonExecution(HumanController controller)
		{
			if (isExecuted || !isActive) return false;

			isExecuted = true;
			return true;
		}
		

	}
}