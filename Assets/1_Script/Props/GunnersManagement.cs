using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HumanFactory.Manager;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace HumanFactory.Controller
{
    public class GunnersManagement : MonoBehaviour
    {
        private GameObject gunnerPrefab;
        private List<Gunner> gunners = new List<Gunner>();
        public List<Gunner> Gunners { get => gunners; set => gunners = value; }

        public void PlaceGunners(Vector2Int mapSize)
        {
            gunnerPrefab = Resources.Load<GameObject>("Prefab/Gunner");

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
            if (humanPos.x >= MapManager.Instance.ProgramMap.GetLength(0)) //오른쪽 탈출
            {
                gunners[1].Shoot();
                gunners[3].Shoot();
            }
            else if (humanPos.y >= MapManager.Instance.ProgramMap.GetLength(1)) //위쪽 탈출
            {
                gunners[0].Shoot();
                gunners[1].Shoot();
            }
            else if (humanPos.x < 0) //왼쪽 탈출
            {
                gunners[0].Shoot();
                gunners[2].Shoot();
            }
            else if (humanPos.y < 0) //아래쪽 탈출
            {
                gunners[2].Shoot();
                gunners[3].Shoot();
            }
        }

    }
}