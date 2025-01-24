using HumanFactory.Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HumanFactory.UI {
    public class CurMouseModeUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI leftMouseInfo;
        [SerializeField] TextMeshProUGUI rightMouseInfo;

        public void ChangeMouseInfo(BuildingType type)
        {
            if (type == BuildingType.None)
            {
                leftMouseInfo.text = "";
                rightMouseInfo.text = "";
            }

            switch (type) {
                case BuildingType.None:
                    break;
            }

        }
    }
}