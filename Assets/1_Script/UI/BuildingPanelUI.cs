using HumanFactory.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HumanFactory.UI {
    public class BuildingPanelUI : MonoBehaviour
    {

        [Header("Content Setter")]
        [SerializeField] private Transform content;
        [SerializeField] private GameObject buildingItemPrefab;
        private Dictionary<int, BuildingPanelItem> items = new Dictionary<int, BuildingPanelItem>();

        private string[] buildingTypes;

        private void Start()
        {
            buildingTypes = Enum.GetNames(typeof(BuildingType));
        }

		public void SetBanner()
        {
            if (MapManager.Instance.CurrentStage < 0) return; 
            int[] buildings = Managers.Resource.GetStageInfo(MapManager.Instance.CurrentStage).enableBuildings;
            for(int i =0; i<buildingTypes.Length-1; i++)
            {
                int idx = Array.IndexOf(buildings, i);
                if (idx < 0)    // 사용가능한 불가능한 건물의 경우
                {
                    // 있으면 없애야됨
                    if (items.ContainsKey(i))
					{
                        items[i].Clear();
						Destroy(items[i].gameObject);
						items.Remove(i);
					}
                }
                else
                {
                    //없으면 만들어야됨
                    if (!items.ContainsKey(i))
                    {
						GameObject go = Instantiate(buildingItemPrefab, content);
                        go.name = i.ToString();
						go.GetComponent<BuildingPanelItem>().SetItemInfo(i);
                        items.Add(i, go.GetComponent<BuildingPanelItem>());
					}
                }
            }

            SortChildrenByName();

		}
		private void SortChildrenByName()
		{
			if (content == null) return;

			Transform[] children = new Transform[content.childCount];
			for (int i = 0; i < content.childCount; i++)
			{
				children[i] = content.GetChild(i);
			}

			System.Array.Sort(children, (x, y) => string.Compare(x.name, y.name));

			for (int i = 0; i < children.Length; i++)
			{
				children[i].SetSiblingIndex(i);
			}
		}
	}
}