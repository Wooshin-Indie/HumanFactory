using System.Collections.Generic;
using UnityEngine;

namespace HumanFactory
{
    public class SaveFileUI : MonoBehaviour
    {
        [SerializeField] private GameObject savefilePrefab;
        [SerializeField] private GameObject addButtonPrefab;

        private List<Transform> items = new List<Transform>();

        private void OnLoadStage(int idx)
        {

        }

        private void OnUnloadStage()
        {
            for(int i=items.Count-1; i>=0; i--)
            {
                Destroy(items[i].gameObject);
            }
        }

    }
}