using HumanFactory.Manager;
using HumanFactory.Util;
using TMPro;
using UnityEngine;

public class Gunner : MonoBehaviour
{
    public void Shoot(Vector2Int humanPos)
    {
        if (humanPos.x >= MapManager.Instance.ProgramMap.GetLength(0)) //오른쪽 탈출
        {

        }
        else if (humanPos.y >= MapManager.Instance.ProgramMap.GetLength(1)) //위쪽 탈출
        {

        }
        else if (humanPos.x < 0) //왼쪽 탈출
        {

        }
        else if (humanPos.y < 0) //아래쪽 탈출
        {

        }
    }

}