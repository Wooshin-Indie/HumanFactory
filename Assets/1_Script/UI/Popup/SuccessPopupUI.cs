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
        [SerializeField] private Button continueButton;
        [SerializeField] private Button goMainButton;
        [SerializeField] private Button nextStageButton;
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
                MapManager.Instance.LoadStage(MapManager.Instance.CurrentStage + 1, 0); // 다음 스테이지 호출
            });
        }

        private Coroutine typeCoroutine = null;
        /// <summary>
        /// 여러가지 정보들을 전달해서 UI에 띄웁니다.
        /// </summary>
        public void SetSuccessPopupInfos(GameResultInfo info)
        {
            RemoveSuccessPopup();

            if (typeCoroutine != null) StopCoroutine(typeCoroutine);
            typeCoroutine = StartCoroutine(SuccessPopupWriting(info, 0.03f));
        }

        public override void PopupWindow()
        {
            base.PopupWindow();
            Managers.Input.LockMouseInput();    // 풀어주는건 OK버튼에서
        }


        private void RemoveSuccessPopup()
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

        private static AudioSource typewritingSource = null;
        private static AudioSource writingSource = null;
        private static AudioSource checkboxSource = null; // 체크박스 체크할 때
        private static AudioSource stampSource = null; // 도장 찍을 때
        public IEnumerator SuccessPopupWriting(GameResultInfo resultInfo, float deltaTime)
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

            for (int i = 0; i < chalTextKeys.Length; i++)
            {
                tmpDescript = LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, chalTextKeys[i]);
                foreach (char letter in tmpDescript.ToCharArray())
                {
                    chalTexts[i].text += letter;
                    yield return new WaitForSeconds(deltaTime);
                }
            }

            typewritingSource.Stop();
            typewritingSource = null;

            yield return new WaitForSeconds(0.5f);

            // 손글쓰기 시작
            writingSource = Managers.Sound.PlaySfx(SFXType.Writing, 1.3f, 1.0f);

            int[] counts = { resultInfo.CycleCount, resultInfo.ButtonCount, resultInfo.KillCount };
            StageInfo stageInfo = Managers.Resource.GetStageInfo(resultInfo.StageIdx);

            bool isChallengePassed = false;
            for (int i = 0; i < 3; i++)
            {
                if (counts[i] <= stageInfo.challenges[i]) isChallengePassed = true;

                tmpDescript = $"{counts[i]}/{stageInfo.challenges[i]}";
                foreach (char letter in tmpDescript.ToCharArray()) // 기록(점수) 출력
                {
                    chalRecords[i].text += letter;
                    yield return new WaitForSeconds(deltaTime);
                }
                if (isChallengePassed)
                {
                    yield return new WaitForSeconds(0.2f);
                    checkboxSource = Managers.Sound.PlaySfx(SFXType.Checkbox, 1.0f, 1.0f); // 체크박스 쓰는 소리
                    checkBoxes[i].sprite = checkedBox;
                }
                isChallengePassed = false;
            }

            tmpDescript = LocalizationSettings.StringDatabase.GetLocalizedString(Constants.TABLE_SUCCESSPOPUPUI, "Success_Comments");
            foreach (char letter in tmpDescript.ToCharArray()) // comment 내용 출력
            {
                comments.text += letter;
                yield return new WaitForSeconds(deltaTime);
            }

            writingSource.Stop();
            writingSource = null;

            yield return new WaitForSeconds(0.5f);


            stampSource = Managers.Sound.PlaySfx(SFXType.Stamp, 1.0f, 1.0f);
            stamp.gameObject.SetActive(true);
            stamp.text = "|COMPLETE|"; // TODO - 도전과제 달성 여부에 따라 스탬프 텍스트 바꿔야함
        }
    }
}