using HumanFactory.Manager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class RemoveButtonUI : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
                {
                    Managers.Input.ChangeCurSelectedBuilding(BuildingType.None);
                });
        }
    }
}