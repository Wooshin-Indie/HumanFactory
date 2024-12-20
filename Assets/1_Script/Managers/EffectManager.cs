using UnityEngine;

namespace HumanFactory.Manager
{
    public class EffectManager
    {

        private GameObject effectRoot;

        private GameObject effectPrefab;

        public void Init()
        {
            effectRoot = GameObject.Find("@EffectManager");
            if (effectRoot == null)
            {
                effectRoot = new GameObject { name = "@EffectManager" };
                UnityEngine.Object.DontDestroyOnLoad(effectRoot);

                // Object Pooling 필요하면 구현
                // 아직 규모가 크지 않아서 필요하진 않은듯?
            }

            // 해당 프리팹에 쓰이는 스프라이트는 ResourcManager에서 로드하고
            // 프리팹은 여기서 로드합니다.

            effectPrefab = Resources.Load<GameObject>("Effects/SpriteEffect");
        }

        public void ShowSpriteEffect(Vector3 pos, EffectType type)
        {
            SpriteEffect effect = GameObject.Instantiate(effectPrefab, pos, Quaternion.identity)
                .GetComponent<SpriteEffect>();

            effect.GetComponent<SpriteRenderer>().sprite = Managers.Resource.GetEffectSprite(type);
            effect.ShowEffect(0.5f, 0.7f);
        }

    }
}