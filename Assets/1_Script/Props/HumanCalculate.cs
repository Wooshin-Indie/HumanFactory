using HumanFactory.Controller;
using HumanFactory.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public static List<HumanController> humanControllers = new List<HumanController>();

    Vector2Int tmpPos = humanControllers[0].CurrentPos;

    // Update is called once per frame
    void Update()
    {
        // 포지션이 겹치는 경우 => tmpList에서 Value가 존재하는 Key에서만 계산 수행
        // operand1의 값에 operand2들의 값을 모두 더함, 그러면서 operand2인 인스턴스들 파괴...   하나 빼고 나머지 삭제(operand2 모두 삭제)

        // 포지션에 특정 발판이 있는 경우,
        // 발판(+1, -1)에 따라 해당 humancontroller의 숫자에 연산처리 후 숫자 업데이트
        // 
        

    }
}
