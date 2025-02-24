using HumanFactory.Buttons;
using UnityEngine;

namespace HumanFactory.Manager
{
	/// <summary>
	/// MapManager의 입력 이벤트 관련 로직들을 담는 partial 클래스
	/// </summary>
	public partial class MapManager
	{
		public BuildingType OnHoverMapGridInNoneMode(int x, int y)
		{
			if (!CheckBoundary(x, y, isMapExpanded) || isDragging)
			{
				tileRect.gameObject.SetActive(false);
				buttonRect.gameObject.SetActive(false);
				return BuildingType.None;
			}

			if (IsCircuiting)
			{
				buttonRect.gameObject.SetActive(true);

				buttonRect.transform.position = new Vector3(circuitingButtonPos.x, circuitingButtonPos.y, Constants.HUMAN_POS_Z);
				tileRect.transform.position = new Vector3(x, y, Constants.HUMAN_POS_Z);
				if (circuitingButtonPos.x == x && circuitingButtonPos.y == y) buttonRect.transform.GetChild(0).gameObject.SetActive(false);
				else
				{
					buttonRect.transform.GetChild(0).gameObject.SetActive(true);
					Vector3 direction = tileRect.transform.position - buttonRect.transform.GetChild(0).position;
					float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
					buttonRect.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, angle - 90f);
				}
				tileRect.gameObject.SetActive(true);

				return BuildingType.None;
			}

			if (programMap[x, y].ButtonBase == null)
			{
				tileRect.gameObject.SetActive(false);
				buttonRect.gameObject.SetActive(false);
				return BuildingType.None;
			}
			
			buttonRect.transform.position = new Vector3(x, y, Constants.HUMAN_POS_Z);
			buttonRect.sprite = programMap[x, y].ButtonBase?.GetComponent<SpriteRenderer>().sprite;
			buttonRect.gameObject.SetActive(true);

			if (programMap[x, y].ButtonBase is TargetableButton)
			{
				tileRect.transform.position = new Vector3(programMap[x, y].ButtonBase.buttonInfo.linkedGridPos.x, programMap[x, y].ButtonBase.buttonInfo.linkedGridPos.y, Constants.HUMAN_POS_Z);
				tileRect.gameObject.SetActive(true);

				if (circuitingButtonPos.x == x && circuitingButtonPos.y == y) buttonRect.transform.GetChild(0).gameObject.SetActive(false);
				else
				{
					buttonRect.transform.GetChild(0).gameObject.SetActive(true);
					Vector3 direction = tileRect.transform.position - buttonRect.transform.GetChild(0).position;
					float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
					buttonRect.transform.GetChild(0).rotation = Quaternion.Euler(0, 0, angle - 90f);
				}
			}
			else
			{
				tileRect.gameObject.SetActive(false);

				buttonRect.transform.GetChild(0).gameObject.SetActive(false);
			}
			return programMap[x, y].ButtonType;
		}

		// 연결 가능한 조합 (order sensitive)
		int[,] linkableIndices = new int[8, 2]
		{
			{0, 1},
			{0, 2},
			{1, 3},
			{2, 3},
			{0, 0},
			{1, 1},
			{2, 2},
			{3, 3},
		};

		// 다른 Grid인지,
		// 해당 linkedPos를 참조하고있는 다른 버튼이 있는지 확인합니다.
		private bool IsAbleToLink(Vector2Int start, Vector2Int end)
		{
			int quad1 = GetMapIdxFromPos(start.x, start.y, IsMapExpanded);
			int quad2 = GetMapIdxFromPos(end.x, end.y, IsMapExpanded);

			if (programMap[circuitingButtonPos.x, circuitingButtonPos.y].ButtonBase is not IJumpable or null)
			{
				return quad1 == quad2;
			}

			if (quad1 < 0 || quad2 < 0) return false;


			for (int i = 0; i < linkableIndices.GetLength(0); i++)
			{
				if (quad1 == linkableIndices[i, 0] && quad2 == linkableIndices[i, 1]) return true;
			}

			GameManagerEx.Instance.DisplayLogByKey("Link_Fail_0", Color.red);
			return false;
		}

		public void OnLeftClickMapGridInNoneMode(int x, int y, bool isSet)
		{
			if (!CheckBoundary(x, y, isMapExpanded, currentMapIdx))
			{
				if (IsCircuiting)
				{
					Managers.Sound.PlaySfx(SFXType.Beep);
					GameManagerEx.Instance.DisplayLogByKey("Link_Fail_1", Constants.COLOR_RED);
				}
				circuitingButtonPos = new Vector2Int(-1, -1);
				return;
			}
			if (IsCircuiting == isSet) return;

			if (IsCircuiting)   // 회로작업 중이면 -> 클릭했을 때 전에 클릭했던 버튼과 연결 
			{
				if (IsAbleToLink(circuitingButtonPos, new Vector2Int(x, y)))
				{
					var targetable = programMap[circuitingButtonPos.x, circuitingButtonPos.y].ButtonBase as TargetableButton;
					targetable?.SetLinkedPos(x, y);
					GameManagerEx.Instance.DisplayLogByKey("Link_Success", Constants.COLOR_CLEARSTAGE);
					Managers.Sound.PlaySfx(SFXType.LinkSuccess);
				}
				circuitingButtonPos = new Vector2Int(-1, -1);
			}
			else
			{
				programMap[x, y].ButtonBase?.OnButtonLeftClick(ref circuitingButtonPos);
			}
		}

		public void OnRightClickMapGridInNoneMode(int x, int y)
		{
			if (IsCircuiting)       // circuiting 중이면 취소
			{
				circuitingButtonPos = new Vector2Int(-1, -1);
				return;
			}

			if (!CheckBoundary(x, y, isMapExpanded, currentMapIdx)) return;
			programMap[x, y].ButtonBase?.OnButtonRightClick();
		}

		private Vector2Int prevDirPad = new Vector2Int(-1, -1);
		private int prevMapIdx = 0;
		public void OnClickMapGridInPadMode(int x, int y)
		{
			if (prevDirPad.x == x && prevDirPad.y == y) return;

			if (!CheckBoundary(prevDirPad.x, prevDirPad.y, isMapExpanded, currentMapIdx))
			{
				if (CheckBoundary(x, y, isMapExpanded, currentMapIdx))
				{
					programMap[x, y].OnClickRotate();
					prevDirPad.Set(x, y);
					return;
				}
				else
				{
					return;
				}
			}

			Vector2Int dir = new Vector2Int(x, y) - prevDirPad;

			PadType type = PadType.DirNone;
			if (dir.x == 1)
			{
				type = PadType.DirRight;
			}
			else if (dir.x == -1)
			{
				type = PadType.DirLeft;
			}
			else if (dir.y == 1)
			{
				type = PadType.DirUp;
			}
			else if (dir.y == -1)
			{
				type = PadType.DirDown;
			}

			programMap[prevDirPad.x, prevDirPad.y].SetPad(type);
			if (CheckBoundary(x, y, isMapExpanded, currentMapIdx)) programMap[x, y].SetPad(type);

			prevDirPad.Set(x, y);
		}
		public void OnReleaseMapGridInPadMode()
		{
			prevDirPad = new Vector2Int(-1, -1);
		}

		public void OnRightClickMapGridInPadMode(int x, int y)
		{
			if (!CheckBoundary(x, y, isMapExpanded, currentMapIdx)) return;

			programMap[x, y].SetPad(PadType.DirNone);
		}

		public void OnHoverMapGridInBuildingMode(int x, int y, BuildingType type)
		{
			if (!CheckBoundary(x, y, isMapExpanded, currentMapIdx))
			{
				ShowDisablePreview(x, y, type);
			}
			else
			{
				ShowEnablePreview(x, y, type);
			}
		}

		public void OnLeftClickMapGridInBuildingMode(int x, int y, BuildingType type)
		{
			if (!CheckBoundary(x, y, isMapExpanded, currentMapIdx)) return;
			Managers.Sound.PlaySfx(SFXType.Button_Put);
			programMap[x, y].SetButton(type);
			Managers.Input.OnInputModeChanged(InputMode.None);
		}

		public void OnInputModeChanged(InputMode mode)
		{
			HidePreview();
			buttonRect.gameObject.SetActive(false);
			tileRect.gameObject.SetActive(false);
			prevDirPad.Set(-1, -1);
			ChangeMapVisibility(mode);
		}

		private void ShowEnablePreview(int x, int y, BuildingType type)
		{
			previewSprite.gameObject.SetActive(true);
			previewSprite.transform.position = new Vector3(x, y, Constants.HUMAN_POS_Z);
			previewSprite.sprite = Managers.Resource.GetBuildingSprite(type, false, true);
			previewSprite.color = Constants.COLOR_INVISIBLE;
		}
		private void ShowDisablePreview(int x, int y, BuildingType type)
		{
			previewSprite.gameObject.SetActive(true);
			previewSprite.transform.position = new Vector3(x, y, Constants.HUMAN_POS_Z);
			previewSprite.sprite = Managers.Resource.GetBuildingSprite(type, false, true);
			previewSprite.color = Color.black;
		}

		private void HidePreview()
		{
			previewSprite.gameObject.SetActive(false);
		}

		private bool isDragging = false;
		public bool IsDragging { get => isDragging; }

		[Header("Input")]
		[SerializeField] private float dragDelay;
		private float elapsedDragTime = 0f;
		private ButtonBase draggingButtonBase;

		public void OnDragStart(Vector2Int mousePos)
		{
			if (!CheckBoundary(mousePos.x, mousePos.y, isMapExpanded)) return;
			if (programMap[mousePos.x, mousePos.y].ButtonBase == null) return;

			draggingButtonBase = programMap[mousePos.x, mousePos.y].ButtonBase;
			draggingButtonBase.OnBeginDrag(mousePos);
		}

		public void OnDrag(Vector2 mouseRowPos)
		{
			if (draggingButtonBase == null) return;

			if (!isDragging)
			{
				elapsedDragTime += Time.deltaTime;
				if (elapsedDragTime > dragDelay)
				{
					elapsedDragTime = 0f;
					isDragging = true;
				}
				return;
			}
			draggingButtonBase?.OnDrag(mouseRowPos);
		}

		// prevPos 와 mousePos 는 항상 다름		
		public void OnDragEnd(Vector2Int prevPos, Vector2Int mousePos)
		{
			if (draggingButtonBase == null) return;

			programMap[prevPos.x, prevPos.y].ButtonBase = null;
			if (!CheckBoundary(mousePos.x, mousePos.y, isMapExpanded))
			{
				Managers.Sound.PlaySfx(SFXType.Button_Remove, .8f);
				Destroy(draggingButtonBase.gameObject);
				Managers.Sound.PlaySfx(SFXType.Beep);
				return;
			}

			if (programMap[mousePos.x, mousePos.y].ButtonBase != null)
			{
				programMap[mousePos.x, mousePos.y].SetButton(BuildingType.None);
			}
			Managers.Sound.PlaySfx(SFXType.Button_Put);
			programMap[mousePos.x, mousePos.y].ButtonBase = draggingButtonBase;
			draggingButtonBase.OnEndDrag(mousePos);
		}

		public void OnClearDrag()
		{
			elapsedDragTime = 0f;
			isDragging = false;
			draggingButtonBase = null;
		}
	}
}