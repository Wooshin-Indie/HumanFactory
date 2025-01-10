using HumanFactory.Manager;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class CurrentModeUI : MonoBehaviour
    {
        [SerializeField] private List<Transform> modes;

        

        private void Start()
        {
            Managers.Input.OnModeChangedAction -= SetMode;
            Managers.Input.OnModeChangedAction += SetMode;

            SetMode(InputMode.None);
            gameObject.SetActive(false);
        }

        public void SetMode(InputMode mode)
        {
            for (int i = 0; i < modes.Count; i++)
            {
                ChangeMode(i, (int)mode == i);
            }
        }


        private void ChangeMode(int index, bool isActive)
        {
            if (isActive)
            {
                modes[index].GetComponent<Image>().color = Constants.COLOR_WHITE;
                modes[index].GetChild(0).GetComponent<TextMeshProUGUI>().color = Constants.COLOR_WHITE;
            }
            else
            {
                modes[index].GetComponent<Image>().color = Constants.COLOR_INVISIBLE;
                modes[index].GetChild(0).GetComponent<TextMeshProUGUI>().color = Constants.COLOR_INVISIBLE;
            }
        }

    }
}