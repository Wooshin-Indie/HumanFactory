using HumanFactory.Manager;
using System.Linq;
using TMPro;
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

	}
}