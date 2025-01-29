using HumanFactory.Manager;
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
            gunners.Add(Instantiate(gunnerPrefab, new Vector3(-1f, mapSize.y - 0.5f, Constants.HUMAN_POS_Z), Quaternion.identity).
                GetComponent<Gunner>()); //왼쪽 위
			gunners[0].gameObject.GetComponent<SpriteRenderer>().flipX = true;

			gunners.Add(Instantiate(gunnerPrefab, new Vector3(mapSize.x, mapSize.y - 0.5f, Constants.HUMAN_POS_Z), Quaternion.identity).
                GetComponent<Gunner>()); //오른쪽 위

			gunners.Add(Instantiate(gunnerPrefab, new Vector3(-1f, -1f, Constants.HUMAN_POS_Z), Quaternion.identity).
                GetComponent<Gunner>()); //왼쪽 아래
			gunners[2].gameObject.GetComponent<SpriteRenderer>().flipX = true;

			gunners.Add(Instantiate(gunnerPrefab, new Vector3(mapSize.x, -1f, Constants.HUMAN_POS_Z), Quaternion.identity).
                GetComponent<Gunner>()); //오른쪽 아래
		}

        public void DetectEscaped(Vector2Int humanPos)
        {

			if (humanPos.x >= MapManager.Instance.ProgramMap.GetLength(0)) //오른쪽 탈출
			{
				gunners[1].Shoot(false);
				gunners[3].Shoot(false);
			}
			else if (humanPos.y >= MapManager.Instance.ProgramMap.GetLength(1)) //위쪽 탈출
			{
				gunners[0].Shoot(false);
				gunners[1].Shoot(true);
			}
			else if (humanPos.x < 0) //왼쪽 탈출
			{
				gunners[0].Shoot(true);
				gunners[2].Shoot(true);
			}
			else if (humanPos.y < 0) //아래쪽 탈출
			{
				gunners[2].Shoot(false);
				gunners[3].Shoot(true);
			}
        }

    }
}