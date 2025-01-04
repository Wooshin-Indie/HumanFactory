using UnityEngine;
using DG.Tweening;

public class SpriteEffect : MonoBehaviour
{
    public void ShowEffect(float localY, float duration)
    {
        Debug.Log(transform.position.y + localY);
        transform.DOMoveY(transform.position.y + localY, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                GameObject.Destroy(gameObject);
            });

    }

    private void Update()
    {
        Debug.Log(transform.position.ToString());
    }
}
