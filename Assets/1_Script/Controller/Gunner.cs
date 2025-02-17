using HumanFactory;
using HumanFactory.Manager;
using HumanFactory.Util;
using UnityEngine;

public class Gunner : MonoBehaviour
{
    public void Shoot(bool isLeft)
    {
        ResetState(stdPos);

        if (GetComponent<Animator>().GetBool(Constants.ANIM_PARAM_FIRE)) return;

        GetComponent<SpriteRenderer>().flipX = isLeft;
        GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_FIRE);
    }

    public void TriggerGunSound()
    {
        Managers.Sound.PlaySfx(SFXType.Shot, 0.3f, 1.2f);
    }

    bool isWalking;
    float walkTime;
    float coolTime;
    float duration;
    float walkSpeed = 0.3f;
    float distance;
    Vector2 stdPos; // 맨 처음 시작 위치
    Vector2 prevPos; // 이전 위치
    Vector2 targetPos; // 다음 위치
    public void ResetState(Vector2 stdPos)
    {
        isWalking = false;
        walkTime = 0f;
        coolTime = 0f;
        duration = 0f;
        prevPos = transform.position;
        this.stdPos = stdPos;
        GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_IDLE);
        GetComponent<Animator>().Play("Gunner_Idle_Clip");

        tmpAni = GetComponent<Animator>().runtimeAnimatorController;
        for (int i = 0; i < tmpAni.animationClips.Length; i++)
        {
            if (tmpAni.animationClips[i].name == "walk")
            {
                tmpClipLength = tmpAni.animationClips[i].length;
                return;
            }
        }
    }

    RuntimeAnimatorController tmpAni;
    float tmpClipLength = 0;
    float tmpTime = 0f;
    public void OnUpdate()
    {
        if (isWalking)
        {
            duration += Time.deltaTime / MapManager.Instance.CycleTime;

            tmpTime = Mathf.Clamp((duration - tmpClipLength), 0, walkTime) / walkTime;

            if (duration < tmpClipLength) return;
            
            if (duration - tmpClipLength > walkTime)
            {
                duration = 0f;
                prevPos = targetPos;
                isWalking = false;
                GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_IDLE);
            }
            else
            {
                Vector2 tmpPos = Vector2.Lerp(prevPos, targetPos, tmpTime);
                transform.position = new Vector3(tmpPos.x, tmpPos.y, Constants.HUMAN_POS_Z);
            }
        }
        else // 안걸을 때 쿨타임 돌리다가 다시 걷기 시작
        {
            coolTime = UnityEngine.Random.Range(5f, 10f);
            duration += Time.deltaTime / MapManager.Instance.CycleTime;

            if (duration > coolTime)
            {
                targetPos = new Vector2(UnityEngine.Random.Range(stdPos.x - 1f, stdPos.x + 1f), UnityEngine.Random.Range(stdPos.y - 1f, stdPos.y + 1f));
                distance = Vector2.Distance(prevPos, targetPos);

                bool isLeft;
                isLeft = ((targetPos.x - prevPos.x) < 0) ? true : false;

                if (isLeft) GetComponent<SpriteRenderer>().flipX = true;
                else GetComponent<SpriteRenderer>().flipX = false;

                walkTime = distance / walkSpeed;

                isWalking = true;
                GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_WALK);
                duration = 0f;
            }
        }
    }
}