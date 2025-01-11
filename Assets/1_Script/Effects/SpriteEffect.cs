using UnityEngine;
using DG.Tweening;

public class SpriteEffect : MonoBehaviour
{
    public void ShowEffect(float localY, float duration)
    {
        transform.DOMoveY(transform.position.y + localY, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                GameObject.Destroy(gameObject);
            });

    }

}
