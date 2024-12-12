using System.Collections;
using TMPro;
using UnityEngine;

namespace HumanFactory.Util.Effect
{
    public static class TypingEffect
    {
        public static IEnumerator TypingCoroutine(TextMeshProUGUI tmp, string str, float deltaTime)
        {
            tmp.text = "";

            foreach (char letter in str.ToCharArray())
            {
                tmp.text += letter;
                yield return new WaitForSeconds(deltaTime);
            }
        }
    }
}