using UnityEngine;
using HumanFactory.Manager;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using System.Collections;
using System.Collections.Generic;
using System;

namespace HumanFactory.UI
{
    public class SuccessPopupUI : PopUpUIBase
    {
        [Header("Buttons")]
        [SerializeField] private List<Button> buttons = new List<Button>();
        [Header("PopupScripts")]
        [SerializeField] TextMeshProUGUI titleText;
        [SerializeField] TextMeshProUGUI areaText;
        [SerializeField] TextMeshProUGUI recordsText;
        [SerializeField] TextMeshProUGUI commentText;
        [SerializeField] TextMeshProUGUI comments;
        [SerializeField] private List<TextMeshProUGUI> chalTexts = new List<TextMeshProUGUI>();
        [SerializeField] private List<TextMeshProUGUI> chalRecords = new List<TextMeshProUGUI>();
        [SerializeField] private List<Image> checkBoxes = new List<Image>();
        [SerializeField] TextMeshProUGUI stamp;
        [Header("Images")]
        [SerializeField] Sprite checkedBox;
        [SerializeField] Sprite emptyBox;

        public override void Awake()
        {
            base.Awake();
            buttons[0].onClick.AddListener(() => // Continue Button
            {
                Managers.Input.ReleaseMouseInput();
                CloseWindow();
            });
            buttons[1].onClick.AddListener(() => // Main Menu Button
            {
                Managers.Input.ReleaseMouseInput();
                CloseWindow();
                Managers.Input.OnEscape();
            });
        }

        private void Update()
        {
            //if (!GetComponent<Button>().IsActive()) return;

            //if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
            //{
            //    GetComponent<Button>().onClick.Invoke();
            //}
        }

		private Coroutine typeCoroutine = null;
        /// <summary>
        /// 여러가지 정보들을 전달해서 UI에 띄웁니다.
        /// </summary>
        public void SetSuccessPopupInfos(GameResultInfo resultInfo)
        {
            RemoveContents();
            InactivateButtons();

            if (typeCoroutine != null) StopCoroutine(typeCoroutine);
            typeCoroutine = StartCoroutine(ContentsWritingEffect(resultInfo, 0.03f));

            GetComponent<Button>().enabled = true;
            GetComponent<Button>().onClick.AddListener(() =>
            {
                StopCoroutine(typeCoroutine);
                typewritingSource?.Stop();
                handwritingSource?.Stop();

                SetContentsAnOnce(resultInfo);
                GetComponent<Button>().enabled = false;
            });
        }

        public override void PopupWindow()
        {
            base.PopupWindow();
            Managers.Input.LockMouseInput();    // 풀어주는건 OK버튼에서
        }


        private void RemoveContents() // string 공백으로 초기화
        {
            titleText.text = "";
            areaText.text = "";
            recordsText.text = "";
            for (int i = 0; i < 3; i++)
            {
                chalTexts[i].text = "";
            }
            for (int i = 0; i < 3; i++)
            {
                chalRecords[i].text = "";
            }
            for (int i = 0; i < 3; i++)
            {
                checkBoxes[i].sprite = emptyBox;
            }
            commentText.text = "";
            comments.text = "";
            stamp.gameObject.SetActive(false);
        }

        private void ActivateButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].gameObject.SetActive(true);
            }
        }

        private void InactivateButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        private static AudioSource typewritingSource = null;
        private static AudioSource handwritingSource = null;
        private static AudioSource checkboxSource = null; // 체크박스 체크할 때
        public IEnumerator ContentsWritingEffect(GameResultInfo resultInfo, float deltaTime)
        {
            string[] chalTextKeys =
                {"Success_Chal0Text", "Success_Chal1Text", "Success_Chal2Text",};

            typewritingSource?.Stop();
            typewritingSource = Managers.Sound.PlaySfx(SFXType.Typewriting, 1.3f, 1.0f);

            titleText.text = "";
            string tmpDescript = resultInfo.ChapterIdx.ToString() + " - " + resultInfo.StageIdx.ToString()
                + LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Success_TitleText");
            foreach (char letter in tmpDescript.ToCharArray()) // 타이틀 출력
            {
                titleText.text += letter;
                yield return new WaitForSeconds(deltaTime);
            }

            tmpDescript = LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Success_AreaText")
                + resultInfo.ChapterIdx.ToString() + " - " + resultInfo.StageIdx.ToString();
            foreach (char letter in tmpDescript.ToCharArray()) // Area 항목 머리말 출력
            {
                areaText.text += letter;
                yield return new WaitForSeconds(deltaTime);
            }

            tmpDescript = LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Success_RecordsText");
            foreach (char letter in tmpDescript.ToCharArray()) // Records 항목 머리말 출력
            {
                recordsText.text += letter;
                yield return new WaitForSeconds(deltaTime);
            }

            for (int i = 0; i < chalTextKeys.Length; i++) // 각 challenge 머리말 출력
            {
                tmpDescript = LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, chalTextKeys[i]);
                foreach (char letter in tmpDescript.ToCharArray())
                {
                    chalTexts[i].text += letter;
                    yield return new WaitForSeconds(deltaTime);
                }
            }

            tmpDescript = LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Success_CommentText");
            foreach (char letter in tmpDescript.ToCharArray()) // Comment 항목 머리말 출력
            {
                commentText.text += letter;
                yield return new WaitForSeconds(deltaTime);
            }

            typewritingSource.Stop();
            typewritingSource = null;

            yield return new WaitForSeconds(0.5f);

            // 손글쓰기 시작
            handwritingSource = Managers.Sound.PlaySfx(SFXType.Writing, 1.3f, 1.0f);

            int[] counts = { resultInfo.CycleCount, resultInfo.ButtonCount, resultInfo.KillCount };
            StageInfo stageInfo = Managers.Resource.GetStageInfo(resultInfo.StageIdx);

            bool isChallengePassed = false;
            for (int i = 0; i < 3; i++)
            {
                tmpDescript = $"{counts[i]}/{stageInfo.challenges[i]}";
                foreach (char letter in tmpDescript.ToCharArray()) // 기록(점수) 출력
                {
                    chalRecords[i].text += letter;
                    yield return new WaitForSeconds(deltaTime);
                }
                if (counts[i] <= stageInfo.challenges[i])
                {
                    yield return new WaitForSeconds(0.2f);
                    checkboxSource = Managers.Sound.PlaySfx(SFXType.Checkbox, 1.0f, 1.0f); // 체크박스 쓰는 소리
                    checkBoxes[i].sprite = checkedBox;
                }
                else checkBoxes[i].sprite = emptyBox;
            }

			String commandText = "";
			commandText += LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Comment_Cycle_" +
				(resultInfo.CycleCount <= stageInfo.challenges[0] ? "Success" : "Fail")) + '\n';
			commandText += LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Comment_Button_" +
				(resultInfo.ButtonCount <= stageInfo.challenges[1] ? "Success" : "Fail")) + '\n';
			commandText += LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Comment_Kill_" +
				(resultInfo.KillCount <= stageInfo.challenges[2] ? "Success" : "Fail"));
            tmpDescript = commandText;
			foreach (char letter in tmpDescript.ToCharArray()) // comment 내용 출력
            {
                comments.text += letter;
                yield return new WaitForSeconds(deltaTime);
            }

            handwritingSource.Stop();
            handwritingSource = null;

            yield return new WaitForSeconds(0.5f);


            ActivateStamp(resultInfo);
            this.GetComponent<Button>().enabled = false;

            yield return new WaitForSeconds(0.3f);

            ActivateButtons();
        }

        private void SetContentsAnOnce(GameResultInfo resultInfo)
        {
            // 타이틀 출력
            titleText.text = resultInfo.ChapterIdx.ToString() + " - " + resultInfo.StageIdx.ToString() 
                + LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Success_TitleText");

            // Area 항목 머리말 출력
            areaText.text = LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Success_AreaText")
                + resultInfo.ChapterIdx.ToString() + " - " + resultInfo.StageIdx.ToString();

            // Records 항목 머리말 출력
            recordsText.text = LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Success_RecordsText");

            // 각 challenge 머리말 출력
            string[] chalTextKeys =
                {"Success_Chal0Text", "Success_Chal1Text", "Success_Chal2Text",};
            for (int i = 0; i < chalTextKeys.Length; i++)
            {
                chalTexts[i].text = LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, chalTextKeys[i]);
            }

            // Comment 항목 머리말 출력
            commentText.text = LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Success_CommentText");

            int[] counts = { resultInfo.CycleCount, resultInfo.ButtonCount, resultInfo.KillCount };
            StageInfo stageInfo = Managers.Resource.GetStageInfo(resultInfo.StageIdx);

            for (int i = 0; i < 3; i++)  // 기록(점수) 출력 및 체크박스 활성화
            {
                chalRecords[i].text = $"{counts[i]}/{stageInfo.challenges[i]}";
                checkBoxes[i].sprite = (counts[i] <= stageInfo.challenges[i]) ? checkedBox : emptyBox;
            }

            // Comments 내용 출력
            String commandText = "";
            commandText += LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Comment_Cycle_" +
				(resultInfo.CycleCount <= stageInfo.challenges[0] ? "Success" : "Fail")) + '\n';
			commandText += LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Comment_Button_" +
				(resultInfo.ButtonCount <= stageInfo.challenges[1] ? "Success" : "Fail")) + '\n';
			commandText += LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Comment_Kill_" +
				(resultInfo.KillCount <= stageInfo.challenges[2] ? "Success" : "Fail"));

			comments.text = commandText;

            ActivateStamp(resultInfo);
            ActivateButtons();
        }

        private static AudioSource stampSource = null; // 도장 찍을 때
        private void ActivateStamp(GameResultInfo resultInfo)
        {
            StageInfo stageInfo = Managers.Resource.GetStageInfo(resultInfo.StageIdx);

            stampSource = Managers.Sound.PlaySfx(SFXType.Stamp, 1.0f, 1.0f);
            stamp.gameObject.SetActive(true);

            int chalCount = 0;
            if (resultInfo.CycleCount <= stageInfo.challenges[0]) chalCount++;
            if (resultInfo.ButtonCount <= stageInfo.challenges[1]) chalCount++;
            if (resultInfo.KillCount <= stageInfo.challenges[2]) chalCount++;

            switch (chalCount) {
                case 0:
					stamp.text = "|PASS|";
					break;
                case 1:
					stamp.text = "|GOOD|";
					break;
                case 2:
					stamp.text = "|GREAT|";
					break;
                case 3:
					stamp.text = "|EXCELLENT!|";
					break;
            }

        }


		public override void OnDisable()
		{
			GameManagerEx.Instance.ChangeRenderCamera(GameManagerEx.Instance.CurrentCamType);
		}
	}
}