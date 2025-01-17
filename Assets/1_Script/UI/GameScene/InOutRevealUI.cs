using HumanFactory.Manager;
using System.Linq;
using UnityEngine;

namespace HumanFactory.UI
{
	public class InOutRevealUI : MonoBehaviour
	{
		[SerializeField] private StackRevealUI inStack;
		[SerializeField] private StackRevealUI outStack;

		private void OnEnable()
		{
			inStack.Clear();
			outStack.Clear();

			for (int i = 0; i < MapManager.Instance.CurrentStageInfo.inputs.Count(); i++)
				inStack.PushValue(MapManager.Instance.CurrentStageInfo.inputs[i]);
			for (int i = 0; i < MapManager.Instance.CurrentStageInfo.outputs.Count(); i++)
				outStack.PushValue(MapManager.Instance.CurrentStageInfo.outputs[i]);
		}


		public void OnClear()
		{
			inStack.InitColors();
			outStack.InitColors();
		}

		public void SetValue(int idx, bool isInput, int value = 0)
		{
			if (isInput)
			{
				inStack.SetInput(isInput, value, idx);
			}
			else
			{
				outStack.SetInput(isInput, value, idx);
			}
		}
	}
}