using HumanFactory.Manager;
using System.Collections;
using System.Transactions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HumanFactory.UI {
    public class ButtonSetUI : PopUpUIBase,
        IPointerEnterHandler, 
        IPointerExitHandler
    {
        [Header("Buttons")]
        [SerializeField] private Button destSetButton;
        [SerializeField] private Button btnTypeLeft;
        [SerializeField] private Button btnTypeRight;
        [SerializeField] private Button opTypeRight;
        [SerializeField] private Button opTypeLeft;

        [SerializeField] private TextMeshProUGUI btnTypeText;
        [SerializeField] private TextMeshProUGUI opTypeText;

        private Vector2 originDelta = new Vector2(600, 450);
        private Vector2 shortDelta = new Vector2(600, 220);

        // GridInfos
        private MapGrid currentGridInfo;

        public override void Awake()
        {

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Managers.Input.LockMouseInput();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Managers.Input.ReleaseMouseInput();
        }

        public override void Start()
        {
            Managers.Input.OnClickMapGridInNoneModeAction = OnClickMapGridInNoneMode;
            Managers.Input.OnModeChangedAction -= OnModeChanged;
            Managers.Input.OnModeChangedAction += OnModeChanged;

            destSetButton.onClick.AddListener(() =>
            {
                MapManager.Instance.OnClickMapGridInNoneMode(currentGridInfo.PosX, currentGridInfo.PosY, true);
                CloseWindow();
            });
            btnTypeLeft.onClick.AddListener(() =>
            {
                currentGridInfo.ButtonInfo.ChangeButtonType(false);
                UpdateUIInfo();
            });
            btnTypeRight.onClick.AddListener(() =>
            {
                currentGridInfo.ButtonInfo.ChangeButtonType(true);
                UpdateUIInfo();
            });
            opTypeLeft.onClick.AddListener(() =>
            {
                OnOpType(false);
            });
            opTypeRight.onClick.AddListener(() =>
            {
                OnOpType(true);
            });


            base.Awake();
        }



        private void OnModeChanged(InputMode mode)
        {
            if (gameObject.activeSelf)
                CloseWindow();
        }

        // Pos 를 눌렀을 떄, Map에서 뭐가 있는지 확인하고
        // Button이 있으면 정보 업데이트, UI띄움
        // Button이 아니면 or OutOfBound 면 켜져있어도 끔
        private void OnClickMapGridInNoneMode(Vector2Int pos)
        {
            if (!MapManager.Instance.CheckBoundary(pos.x, pos.y))
            {
                CloseWindow();
                return;
            }

            currentGridInfo = MapManager.Instance.GetMapGrid(pos.x, pos.y);

            if (currentGridInfo.BuildingType == BuildingType.Button ||
                currentGridInfo.BuildingType == BuildingType.Jump)
            {
                gameObject.SetActive(true);
                UpdateUIInfo();
            }
            else
            {
                CloseWindow();
            }

            return;
        }


        private void OnOpType(bool isNext)
        {
            switch (currentGridInfo.ButtonInfo.buttonType)
            {
                case ButtonType.Input:
                    currentGridInfo.ButtonInfo.ChangeInputType(isNext);
                    break;
                case ButtonType.Rotate:
                    currentGridInfo.ButtonInfo.ChangePadType(isNext);
                    break;
                case ButtonType.Toggle:
                    currentGridInfo.ButtonInfo.ChangeToggleType(isNext);
                    break;
            }
            UpdateUIInfo();
        }


        private void UpdateUIInfo()
        {
            if (currentGridInfo == null) return;

            if (currentGridInfo.BuildingType == BuildingType.Button)
            {
                btnTypeText.gameObject.SetActive(true);
                btnTypeLeft.gameObject.SetActive(true);
                btnTypeRight.gameObject.SetActive(true);
                opTypeText.gameObject.SetActive(true);
                opTypeLeft.gameObject.SetActive(true); 
                opTypeRight.gameObject.SetActive(true);

                GetComponent<RectTransform>().sizeDelta = originDelta;

                btnTypeText.text = currentGridInfo.ButtonInfo.buttonType.ToString();

                switch (currentGridInfo.ButtonInfo.buttonType)
                {
                    case ButtonType.Input:
                        opTypeText.text = currentGridInfo.ButtonInfo.inputType.ToString();
                        break;
                    case ButtonType.Rotate:
                        opTypeText.text = currentGridInfo.ButtonInfo.dirType.ToString();
                        break;
                    case ButtonType.Toggle:
                        opTypeText.text = currentGridInfo.ButtonInfo.toggleType.ToString();
                        break;
                }
            }
            else if(currentGridInfo.BuildingType == BuildingType.Jump)
            {
                btnTypeText.gameObject.SetActive(false);
                btnTypeLeft.gameObject.SetActive(false);
                btnTypeRight.gameObject.SetActive(false);
                opTypeText.gameObject.SetActive(false);
                opTypeLeft.gameObject.SetActive(false);
                opTypeRight.gameObject.SetActive(false);


                GetComponent<RectTransform>().sizeDelta = shortDelta;
            }
            else
            {
                Debug.LogError("Why UI is opened?");
            }

        }


    }
}