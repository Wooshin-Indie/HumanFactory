using System.Collections.Generic;
using UnityEngine;

namespace HumanFactory.Controller
{
    public class GunnersManagement : MonoBehaviour
    {
        [SerializeField] private GameObject gunnerPrefab;
        private List<Gunner> gunners = new List<Gunner>();
        public List<Gunner> Gunners { get => gunners; set => gunners = value; }

        public void PlaceGunners(Vector2Int mapSize)
        {
            gunners.Add(Instantiate(gunnerPrefab, new Vector3(-2f, mapSize.y - 1f, Constants.HUMAN_POS_Z), Quaternion.identity).
                GetComponent<Gunner>()); //왼쪽 위
            gunners[0].gameObject.GetComponent<SpriteRenderer>().flipX = true;

            gunners.Add(Instantiate(gunnerPrefab, new Vector3(mapSize.x + 1f, mapSize.y - 1f, Constants.HUMAN_POS_Z), Quaternion.identity).
                GetComponent<Gunner>()); //오른쪽 위

            gunners.Add(Instantiate(gunnerPrefab, new Vector3(-2f, 0f, Constants.HUMAN_POS_Z), Quaternion.identity).
                GetComponent<Gunner>()); //왼쪽 아래
            gunners[2].gameObject.GetComponent<SpriteRenderer>().flipX = true;

            gunners.Add(Instantiate(gunnerPrefab, new Vector3(mapSize.x + 1f, 0f, Constants.HUMAN_POS_Z), Quaternion.identity).
                GetComponent<Gunner>()); //오른쪽 아래
        }

        public void DetectEscaped(Vector2Int humanPos)
        {
            gunners[0].Shoot(humanPos);
            gunners[1].Shoot(humanPos);
            gunners[2].Shoot(humanPos);
            gunners[3].Shoot(humanPos);
        }

    }
}