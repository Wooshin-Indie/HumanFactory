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

		public Vector2[] expandPos;		// 확장되었을 때, 9명의 위치
		public Vector2[] originPos;     // 확장되지 않을 때, 4명의 위치
        public float[,] originRange;

		private void Update()
		{
			for (int i = 0; i < gunners.Count; i++)
			{
				if (!(gunners[i].gameObject.activeSelf)) return;
				else gunners[i].OnUpdate();
			}
		}

        public void PlaceGunners(Vector2Int mapSize)
        {
			Vector2 interval = MapManager.Instance.MapInterval;
			expandPos = new Vector2[9]
			{
				new Vector2(-2, -1),
				new Vector2(mapSize.x -0.5f + interval.x /2, -1),
				new Vector2(mapSize.x * 2 + interval.x + 1, -1),
				new Vector2(-2, mapSize.y + interval.y/2 -1),
				new Vector2(mapSize.x -0.5f + interval.x /2,  mapSize.y + interval.y/2 -1),
				new Vector2(mapSize.x * 2 + interval.x + 1,  mapSize.y + interval.y/2 -1),
				new Vector2(-2, 2 * mapSize.y + interval.y - 0.5f),
				new Vector2(mapSize.x -0.5f + interval.x /2, 2 * mapSize.y + interval.y - 0.5f),
				new Vector2(mapSize.x * 2 + interval.x + 1, 2 * mapSize.y + interval.y - 0.5f),
			};

			originPos = new Vector2[4]
			{
				new Vector2(-2, -1),
				new Vector2(mapSize.x + 1, -1),
				new Vector2(-2, mapSize.y - 0.5f),
				new Vector2(mapSize.x + 1, mapSize.y-0.5f)
			};

			

            for (int i=0; i<expandPos.Length; i++)
			{
				gunners.Add(Instantiate(gunnerPrefab, new Vector3(expandPos[i].x, expandPos[i].y, Constants.HUMAN_POS_Z), Quaternion.identity).
					GetComponent<Gunner>());
			}
		}

		public void LoadGunners(bool isExpanded)
		{
			if (!isExpanded)
			{
				for (int i = 0; i < gunners.Count; i++)
				{
					if (i < originPos.Length)
					{
						gunners[i].gameObject.SetActive(true);
						gunners[i].transform.position = new Vector3(originPos[i].x, originPos[i].y, Constants.HUMAN_POS_Z);
						gunners[i].ResetState(originPos[i]);
					}
					else
					{
						gunners[i].gameObject.SetActive(false);
					}
				}
			}
			else
			{
				for (int i = 0; i < expandPos.Length; i++)
				{
					gunners[i].gameObject.SetActive(true);
					gunners[i].transform.position = new Vector3(expandPos[i].x, expandPos[i].y, Constants.HUMAN_POS_Z);
                    gunners[i].ResetState(expandPos[i]);
                }
			}
		}

		int[] idxOffset = new int[4] {0, 1, 3, 4};

        public void DetectEscaped(Vector2Int dir, int idx, bool isExpanded)
		{
			if (idx < 0) {
				Debug.LogError("Wrong Idx!");
				return;
			}

			if (isExpanded)
			{
				switch (dir)
				{
					case var _ when dir.x > 0:      // 오른쪽
						gunners[1 + idxOffset[idx]].Shoot(false);
						gunners[4 + idxOffset[idx]].Shoot(false);
						break;
					case var _ when dir.x < 0:      // 왼쪽
						gunners[0 + idxOffset[idx]].Shoot(true);
						gunners[3 + idxOffset[idx]].Shoot(true);
						break;
					case var _ when dir.y > 0:      // 위쪽
						gunners[3 + idxOffset[idx]].Shoot(false);
						gunners[4 + idxOffset[idx]].Shoot(true);
						break;
					case var _ when dir.y < 0:      // 아래쪽
						gunners[0 + idxOffset[idx]].Shoot(false);
						gunners[1 + idxOffset[idx]].Shoot(true);
						break;
				}
			}
			else
			{
				switch (dir)
				{
					case var _ when dir.x > 0:      // 오른쪽
						gunners[1].Shoot(false);
						gunners[3].Shoot(false);
						break;
					case var _ when dir.x < 0:      // 왼쪽
						gunners[0].Shoot(true);
						gunners[2].Shoot(true);
						break;
					case var _ when dir.y > 0:      // 위쪽
						gunners[2].Shoot(false);
						gunners[3].Shoot(true);
						break;
					case var _ when dir.y < 0:      // 아래쪽
						gunners[0].Shoot(false);
						gunners[1].Shoot(true);
						break;
				}
			}
        }

		public void ClearHumans(bool isExpanded)
		{
			if (!isExpanded)
			{
				for (int i = 0; i < originPos.Length; i++)
				{
					gunners[i].Shoot(i % 2 != 0);
				}
			}
			else
			{
				for (int i = 0; i < expandPos.Length; i++)
				{
					gunners[i].Shoot(i % 3 == 2);
				}
			}
		}

		// Gunners anim 속도 변경
		public void SetCycleTime(float cycleTime)
		{
			foreach (Gunner gunner in gunners)
			{
				gunner.GetComponent<Animator>().speed = 1 / cycleTime;
			}
		}

	}
}