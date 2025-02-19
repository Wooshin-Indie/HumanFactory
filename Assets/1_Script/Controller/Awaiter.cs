using HumanFactory;
using HumanFactory.Manager;
using HumanFactory.Util;
using Org.BouncyCastle.Asn1.BC;
using UnityEngine;

public class Awaiter : MonoBehaviour
{
    private bool isWalking;
    private Vector2 targetPos;
    private bool isHead = false;
    public bool IsHead { get => isHead; set => isHead = value; }
    private float checkDot = 1f;
    private Vector2 prevVector;
    private Vector2 nowVector;
    public void OnUpdate()
    {
        if (!isWalking) return;

        if (checkDot > 0)
        {
            prevVector = new Vector2(transform.position.x, transform.position.y) - targetPos;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 1 * Time.deltaTime / MapManager.Instance.CycleTime);
            nowVector = new Vector2(transform.position.x, transform.position.y) - targetPos;

            checkDot = Vector2.Dot(prevVector, nowVector);
        }
        else
        {
            GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_IDLE);
            transform.position = targetPos;
            isWalking = false;

            checkDot = 1f;
        }
    }

    private bool isLeft = false;
    public void WalkNextStep(Vector2 nextPos, bool isRandom = false)
    {
        isWalking = true;
        Vector2 tmpRandom = new Vector2(Random.Range(0f, 0.2f), Random.Range(0f, 0.2f));
        if (isRandom) targetPos = nextPos + tmpRandom;
        else targetPos = nextPos;
        checkDot = 1f;

        GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_WALK);
        isLeft = (targetPos.x-transform.position.x < 0) ? true : false;

        if (isLeft) GetComponent<SpriteRenderer>().flipX = true;
        else GetComponent<SpriteRenderer>().flipX = false;
    }

    public void HeadToEnd(Vector2 nextPos)
    {
        isWalking = false;
        transform.position = nextPos;
        checkDot = 1f;
        GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_IDLE);
    }
}
