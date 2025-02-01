using UnityEngine;
using UnityEngine.Localization.Components;
using TMPro;

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

		public static void SetLocalizedString(this TextMeshProUGUI tmp, string table, string key)
		{
			tmp.GetComponent<LocalizeStringEvent>().StringReference = new UnityEngine.Localization.LocalizedString
			{
				TableReference = table,
				TableEntryReference = key
			};
		}
	}
}