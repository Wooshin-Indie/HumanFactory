using UnityEngine;

namespace HumanFactory.Util
{
	public static class Extentions
	{
		/// <summary>
		/// 모든 bool 값 false로 만들고 param만 true로 바꿔서 anim 실행
		/// </summary>
		public static void TurnState(this Animator anim, string param)
		{
			AnimatorControllerParameter[] parameters = anim.parameters;

			for(int i=0; i<parameters.Length; i++)
			{
				anim.SetBool(parameters[i].name, false);
			}

			anim.SetBool(param, true);
		}
	}
}