using HumanFactory.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    [RequireComponent(typeof(Button))]
    public class ExitButton : MonoBehaviour
    {
		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(() => Managers.Input.OnEscape());
		}
	}
}