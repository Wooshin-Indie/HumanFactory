using HumanFactory;
using HumanFactory.Manager;
using HumanFactory.Util;
using UnityEngine;

public class Gunner : MonoBehaviour
{
    public void Shoot(bool isLeft)
    {
        if (GetComponent<Animator>().GetBool(Constants.ANIM_PARAM_FIRE)) return;

        GetComponent<SpriteRenderer>().flipX = isLeft;
        GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_FIRE);
    }

    public void TriggerGunSound()
    {
        Managers.Sound.PlaySfx(SFXType.Shot, 0.3f, 1.2f);
    }
}