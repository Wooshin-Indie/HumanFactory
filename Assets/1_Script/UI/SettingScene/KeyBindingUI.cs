using HumanFactory.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HumanFactory.UI
{
	/// <summary>
	/// BindingItem 클릭 -> InputManager에서 OnBindingKey 로 바인드할 키 받음
	/// -> 그 키를 DataManager에 넘겨서 중복된 키 처리 및 저장 -> 처리 완료되면 OnUpdateBinding 호출해서 UI에 반영
	/// </summary>
	public class KeyBindingUI : MonoBehaviour
	{
		[SerializeField] private Transform contentParent;
		[SerializeField] private GameObject keyBindingItemPrefab;

		private List<KeyBindingItemUI> keyBindingItems = new List<KeyBindingItemUI>();

		private void Start()
		{
			int len = Enum.GetValues(typeof(ShortcutActionEnum)).Length - 1;
			for (int i = 0; i < len; i++)
			{
				keyBindingItems.Add(Instantiate(keyBindingItemPrefab, contentParent).GetComponent<KeyBindingItemUI>());
			}
			Managers.Data.OnUpdateKeyBindings = OnUpdateBindings;

			OnUpdateBindings();
		}

		
		private void OnUpdateBindings()
		{
			for(int i=0; i<keyBindingItems.Count; i++)
			{
				keyBindingItems[i].OnUpdateBinding((ShortcutActionEnum)i);
			}
		}
	}
}