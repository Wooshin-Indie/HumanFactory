using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HumanFactory.UI
{
	public class StackRevealUI : MonoBehaviour
	{

		[SerializeField] private GameObject itemPrefab;

		private List<GameObject> values = new List<GameObject>();

		// TODO : load 횟수 많아지면 pooling 구현해야될듯
		public void PushValue(int value)
		{
			GameObject go = Instantiate(itemPrefab, transform);
			go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = value.ToString();
			values.Add(go);
		}

		public void Clear()
		{
			for (int i = values.Count - 1; i >= 0; i--)
			{
				Destroy(values[i]);
			}
			values.Clear();
		}
	}
}