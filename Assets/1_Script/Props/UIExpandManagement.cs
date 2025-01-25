using DG.Tweening;
using HumanFactory.Effects;
using HumanFactory.Manager;
using HumanFactory.Util.Effect;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace HumanFactory.UI
{
    public class UIExpandManagement : MonoBehaviour
    {
        private List<GameObject> stages = new List<GameObject>();
        private List<GameObject> chapters = new List<GameObject>();

        [Header("Interact Effects")]
        [SerializeField] private GameObject stagePanelPrefab;
        [SerializeField] private Transform content;
        [SerializeField] private float lerpDuration;

        [Header("Additional Subjects")]
        [SerializeField] private TVNoiseEffect noiseEffect;
        [SerializeField] private TextMeshProUGUI stageDescript;
        [SerializeField] private TextMeshProUGUI titleBanner;

        [Header("Panel Buttons")]
        [SerializeField] private Button ChapterBackButton;
        [SerializeField] private List<Button> saveButtons = new List<Button>();

        private Tweener scrollTweener;
        private float scrollviewHeight;

        private int currentExpandedPanel = -1;
        private int currentSelectedIndex = -1;
        private int currentSaveFileIndex = -1;

        public int CurrentSelectedIndex { get => currentSelectedIndex;
            set
            {
				currentSelectedIndex = value;
                if (currentSelectedIndex < 0)
				{
                    noiseEffect.SetPermanentNoise();
                    for (int i = 0; i < saveButtons.Count; i++)
                    {
                        saveButtons[i].gameObject.SetActive(false);
                    }
				}
                else
				{
					for (int i = 0; i < saveButtons.Count; i++)
					{
						saveButtons[i].gameObject.SetActive(true);
					}
                    Debug.Log("SAVE CHALL : " + Managers.Data.GamePlayData.stageGridDatas[currentSelectedIndex].resultDatas.ToString());
					CurrentSaveFileIndex = 0;
				}
			}
        }

        // 이게 바뀔떄마다 Stage load 됨
        // 스테이지 선택하면 자동으로 0번 SaveFile 로드
        public int CurrentSaveFileIndex { get => currentSaveFileIndex;
            set
            {
                currentSaveFileIndex = value;
                MapManager.Instance.LoadStage(CurrentSelectedIndex, currentSaveFileIndex);
                for (int i = 0; i < saveButtons.Count; i++)
                {
                    saveButtons[i].GetComponent<UIItemBase>().OnSelected(i == currentSaveFileIndex);
                }
            }
        }


		private void Start()
        {
            LoadChaptersOnPanel();
            ChapterBackButton.onClick.AddListener(OnClickBackButton);
            ChapterBackButton.interactable = false;

            for (int i = 0; i < saveButtons.Count; i++)
            {
                int t = i;
                saveButtons[i].onClick.AddListener(() =>
                {
                    noiseEffect.MakeNoise();
					MapManager.Instance.LoadStage(CurrentSelectedIndex, t);
                    CurrentSaveFileIndex = t; 
				});
            }
            CurrentSelectedIndex = -1;
        }

        private void Update()
        {
			// 자동으로 돌아가게 만듦 -> 챕터 구분 구현하느라 잠시 꺼둠
			//if (GameManagerEx.Instance.CurrentCamType == CameraType.Main)
			//{
			//    elapsedTime += Time.deltaTime;
			//    if(maxChangeTime < elapsedTime)
			//    {
			//        elapsedTime = 0;
			//        OnClickStages(tmpStagePoiter);
			//        tmpStagePoiter = (tmpStagePoiter + 1) % Managers.Resource.GetStageCount();
			//    }
			//}

			if (Input.GetKeyDown(KeyCode.Space))
			{
				Managers.Sound.PlaySfx(SFXType.Typing, 1f, 1.2f, 4f);
			}
		}

        private void LoadChaptersOnPanel()
        {
            for (int i = 0; i < Managers.Resource.GetChapterCount(); i++)
            {
                int idx = i;
                chapters.Add(Instantiate(stagePanelPrefab, content));
                chapters[idx].GetComponent<UIOnClickExpand>().SetChapterId(idx);
                chapters[idx].GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnClickChapters(idx);
                });
            }

		}

        private void LoadStagesOnPanel()
        {
            int[] stageIndexes = MapManager.Instance.CurrentChapterInfo.stageIndexes;

            for (int i = 0; i < stageIndexes.Length; i++)
            {
                int idx = i;
                int stageIdx = stageIndexes[idx];
                stages.Add(Instantiate(stagePanelPrefab, content));
                stages[idx].GetComponent<UIOnClickExpand>().SetStageId(stageIdx);
                stages[idx].GetComponent<Button>().onClick.AddListener(() =>
                {
                    OnClickStages(idx, stageIdx);
                });
            }
            currentExpandedPanel = -1;
            CurrentSelectedIndex = -1;
		}

        private void OnClickChapters(int index)
        {
            if (typeCoroutine != null) StopCoroutine(typeCoroutine); // 이거 왜 적은거임?

            // chapters 전부 destroy 및 remove
            ClearChapters();

            MapManager.Instance.LoadChapter(index); // currentChapterInfo 업데이트

            LoadStagesOnPanel(); // 챕터에 해당되는 stage들 인스턴시에이트

            titleBanner.text = Managers.Resource.GetChapterInfo(index).chapterName;
			ChapterBackButton.interactable = true;
            // 뒤로가기 버튼 활성화, 뒤로가기 버튼 누르면 stages 전부 destroy 및 remove, 그리고 LoadChapters, 그리고 BackButton 비활성화
        }

        private void OnClickBackButton()
        {
            ClearStages();
            LoadChaptersOnPanel();

			currentExpandedPanel = -1;
			CurrentSelectedIndex = -1;

			titleBanner.text = "STAGES";
			ChapterBackButton.interactable = false;
        }

        private void OnClickStages(int index, int stageIdx) // index는 패널에 표시되는 리스트에서의 index, stageIdx는 실제 json에서 불러오는 stage의 index
        {
            if (typeCoroutine != null) StopCoroutine(typeCoroutine);
            stageDescript.text = "";

            if (currentExpandedPanel == -1) // 아무것도 확대 안돼있음
            {
                stages[index].GetComponent<UIOnClickExpand>().Expand();
                SetSelectedStageOnCenter(index, lerpDuration);
                currentExpandedPanel = index;
				CurrentSelectedIndex = stageIdx;
			}
            else if (currentExpandedPanel == index) // 이미 확대돼있던걸 클릭 -> 축소
            {
                stages[index].GetComponent<UIOnClickExpand>().Reduce();
                currentExpandedPanel = -1;
				CurrentSelectedIndex = -1;
				scrollTweener.Kill();
            }
            else if (currentExpandedPanel != index) // 확대 돼있는게 있는 상태에서 다른 패널 클릭
            {
                stages[currentExpandedPanel].GetComponent<UIOnClickExpand>().Reduce();
                stages[index].GetComponent<UIOnClickExpand>().Expand();
                SetSelectedStageOnCenter(index, lerpDuration);
                currentExpandedPanel = index;
                CurrentSelectedIndex = stageIdx;

            }
        }

        // 선택된(확장된) 스테이지가 가운데 오도록 합니다.
        // Content 의 AnchoredPosition.y를 움직이며, (0, content.height - scollview.height) 범위를 갖습니다.
        // Expand 한 후에 호출되기 때문에, 각 Item의 height 의 절반만큼 조정해줍니다
        private void SetSelectedStageOnCenter(int index, float duration)
        {
            noiseEffect.MakeNoise();
            float maxPosition = content.GetComponent<RectTransform>().rect.height - scrollviewHeight + 200;
            float setPosition = Mathf.Clamp(Mathf.Abs(stages[index].GetComponent<RectTransform>().anchoredPosition.y) - scrollviewHeight / 2,
                0,
                maxPosition);

            scrollTweener = content.GetComponent<RectTransform>().DOAnchorPosY(setPosition, duration);


            StartTypingDescript(index);
        }

        private Coroutine typeCoroutine = null;
        private void StartTypingDescript(int index)
        {
            stageDescript.GetComponent<RectTransform>().DOAnchorPosY(0f, 0f);
            typeCoroutine = StartCoroutine(TypingEffect.TypingCoroutine(stageDescript, "0", 0.01f));
        }

        private void ClearChapters()
        {
            for (int i = chapters.Count - 1; i >= 0; i--)
            {
                Destroy(chapters[i]);
                chapters.Remove(chapters[i]);
            }
        }

        private void ClearStages()
        {
            for (int i = stages.Count - 1; i >= 0; i--)
            {
                Destroy(stages[i]);
                stages.Remove(stages[i]);
            }
        }
    }
}