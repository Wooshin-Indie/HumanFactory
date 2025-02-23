using HumanFactory.Controller;
using UnityEngine;

namespace HumanFactory.Buttons
{
	public abstract class TargetableButton : ButtonBase
	{
		public void SetLinkedPos(int x, int y)
		{
			buttonInfo.linkedGridPos = new Vector2Int(x, y);
		}

		public override bool OnPressed(bool isToggled)
		{
			if (!base.OnPressed(isToggled)) return false;
			isExecuted = true;
			return true;
		}

		public override void OnButtonLeftClick(ref Vector2Int circuitingPos)
		{
			base.OnButtonLeftClick(ref circuitingPos);
			circuitingPos = new Vector2Int(buttonPos.x, buttonPos.y);	// Targetable은 circuiting 시작
		}

		public override void OnButtonRightClick()
		{
			base.OnButtonRightClick();
		}

	}
}