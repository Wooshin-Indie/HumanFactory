using UnityEngine;
using HumanFactory.Manager;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

namespace HumanFactory.UI
{
    public class SuccessPopupUI : PopUpUIBase
    {

        [SerializeField] private Button continueButton;
        [SerializeField] private Button goMainButton;
        [SerializeField] private Button nextStageButton;
        [SerializeField] TextMeshProUGUI stageInfo;
        [SerializeField] TextMeshProUGUI challenge0;
        [SerializeField] TextMeshProUGUI challenge1;
        [SerializeField] TextMeshProUGUI challenge2;
		[SerializeField] Image checkBox0;
        [SerializeField] Image checkBox1;
        [SerializeField] Image checkBox2;

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
				MapManager.Instance.LoadStage(MapManager.Instance.CurrentStage + 1, 0);	// 다음 스테이지 호출
			});
        }


		/// <summary>
		/// 여러가지 정보들을 전달해서 UI에 띄웁니다.
		/// </summary>
		public void SetStageInfos(GameResultInfo info)
        {
            stageInfo.text = info.ChapterIdx.ToString() + " - " + info.StageIdx.ToString();
            challenge0.text = info.CycleCount + "/" + "100";
            challenge1.text = info.ButtonCount + "/" + "200";
            challenge2.text = info.KillCount + "/" + "300";
			//checkbox 이미지 넣기
        }

		public override void PopupWindow()
		{
			base.PopupWindow();
            Managers.Input.LockMouseInput();    // 풀어주는건 OK버튼에서
		}

	}
}