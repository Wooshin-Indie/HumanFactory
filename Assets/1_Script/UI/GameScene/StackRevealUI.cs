using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
	public class StackRevealUI : MonoBehaviour
	{

		[SerializeField] private GameObject itemPrefab;

		private List<GameObject> values = new List<GameObject>();

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

		public void InitColors()
		{
			for (int i = 0; i < values.Count; i++)
			{
				values[i].GetComponent<Image>().color = Color.white;
				values[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
			}
		}


		public void SetInput(bool isInput, int value, int idx)
		{
			if (isInput)
			{
				values[idx].GetComponent<Image>().color = Color.gray;
				values[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.gray;
			}
			else
			{
				if(Int32.Parse(values[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text) == value)
				{
					// Out 일치하는 경우
					values[idx].GetComponent<Image>().color = Color.green;
					values[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.green;
				}
				else
				{
					values[idx].GetComponent<Image>().color = Color.red;
					values[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
				}
			}
		}
	}
}