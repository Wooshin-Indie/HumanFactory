using System;
using UnityEngine;

namespace HumanFactory.UI {
    public class BuildingPanelUI : MonoBehaviour
    {

        [Header("Content Setter")]
        [SerializeField] private Transform content;
        [SerializeField] private GameObject buildingItemPrefab;

        private string[] buildingTypes;

        private void Start()
        {
            buildingTypes = Enum.GetNames(typeof(BuildingType));
            SetBanner();
        }

        /// <summary>
        /// Start 에서 호출되어야 합니다.
        /// </summary>
        private void SetBanner()
        {
            for(int i =0; i<buildingTypes.Length-1; i++)
            {
                GameObject go = Instantiate(buildingItemPrefab, content);
                go.GetComponent<BuildingPanelItem>().SetItemInfo(i);
            }
        }
    }
}