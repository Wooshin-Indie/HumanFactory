using HumanFactory;
using HumanFactory.Util;
using UnityEngine;

public class Gunner : MonoBehaviour
{
    public void Shoot(bool isLeft)
    {
        GetComponent<SpriteRenderer>().flipX = isLeft;
        GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_FIRE);
    }

}