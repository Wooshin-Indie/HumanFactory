using UnityEngine;
using HumanFactory.Manager;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class SuccessPopupUI : PopUpUIBase
    {

        [SerializeField] private Button continueButton;
        [SerializeField] private Button goMainButton;
        [SerializeField] private Button nextStageButton;

		public override void Awake()
		{
			base.Awake();
			continueButton.onClick.AddListener(() =>
			{
				Managers.Input.ReleaseMouseInput();
				CloseWindow();
			});
			goMainButton.onClick.AddListener(() =>
			{
				Managers.Input.ReleaseMouseInput();
				CloseWindow();
				Managers.Input.OnEscape();
			});
			nextStageButton.onClick.AddListener(() =>
			{
				Managers.Input.ReleaseMouseInput();
				CloseWindow();
				MapManager.Instance.LoadStage(MapManager.Instance.CurrentStage + 1);	// 다음 스테이지 호출
			});
		}

		/// <summary>
		/// 여러가지 정보들을 전달해서 UI에 띄웁니다.
		/// </summary>
		public void SetStageInfos()
        {

        }

		public override void PopupWindow()
		{
			base.PopupWindow();
            Managers.Input.LockMouseInput();    // 풀어주는건 OK버튼에서
		}

	}
}