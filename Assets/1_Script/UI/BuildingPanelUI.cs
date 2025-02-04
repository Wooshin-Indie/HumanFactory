using HumanFactory.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI {
    public class BuildingPanelUI : MonoBehaviour
    {

        [Header("Content Setter")]
        [SerializeField] private Transform content;
        [SerializeField] private GameObject buildingItemPrefab;
        private Dictionary<int, BuildingPanelItem> items = new Dictionary<int, BuildingPanelItem>();

        [Header("MapConverButtons")]
        [SerializeField] private List<Button> convertButtons = new List<Button>();
        // Up, Right, Down, Left 순

        private string[] buildingTypes;

        private void Start()
        {
            buildingTypes = Enum.GetNames(typeof(BuildingType));

            MapManager.Instance.OnCurrentMapIdxAction -= OnCurrnentMapSet;
            MapManager.Instance.OnCurrentMapIdxAction += OnCurrnentMapSet;
           
            for (int i = 0; i < convertButtons.Count; i++)
            {
                int t = i;
                convertButtons[i].onClick.AddListener(() =>
                {
                    MapManager.Instance.CurrentMapIdx = (GetMapIdx(MapManager.Instance.CurrentMapIdx, t));
                });
            }
            MapManager.Instance.CurrentMapIdx = 0;
        }

        private void OnCurrnentMapSet(int idx, bool isMapExpanded)
		{
			for (int i = 0; i < convertButtons.Count; i++)
			{
                convertButtons[i].gameObject.SetActive(false);
			}
            if (!isMapExpanded) return;
			switch (idx)
            {
                case 0:
                    convertButtons[0].gameObject.SetActive(true);
                    convertButtons[1].gameObject.SetActive(true);
                    break;
                case 1:
					convertButtons[0].gameObject.SetActive(true);
					convertButtons[3].gameObject.SetActive(true);
					break;
                case 2:
					convertButtons[1].gameObject.SetActive(true);
					convertButtons[2].gameObject.SetActive(true);
					break;
                case 3:
					convertButtons[2].gameObject.SetActive(true);
					convertButtons[3].gameObject.SetActive(true);
					break;
            }
        }

        private int GetMapIdx(int idx, int dir)
        {
            switch (idx)
			{
				case 0:
                    if (dir == 0) return 2;
                    if (dir == 1) return 1;
					break;
				case 1:
					if (dir == 0) return 3;
					if (dir == 3) return 0;
					break;
				case 2:
					if (dir == 1) return 3;
					if (dir == 2) return 0;
					break;
				case 3:
					if (dir == 2) return 1;
					if (dir == 3) return 2;
					break;
			}

            Debug.LogError("GetMapIdx : Wrong Return value");
            return 0;
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