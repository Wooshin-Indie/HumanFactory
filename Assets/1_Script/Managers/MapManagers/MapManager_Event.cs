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
			if (isCircuiting)
			{
				buttonRect.transform.position = new Vector3(circuitingButtonPos.x,
					circuitingButtonPos.y, Constants.HUMAN_POS_Z);
				tileRect.transform.position = new Vector3(x, y, Constants.HUMAN_POS_Z);
				if (circuitingButtonPos.x != x || circuitingButtonPos.y != y)
					tileRect.gameObject.SetActive(true);
				else
					tileRect.gameObject.SetActive(false);

				return BuildingType.None;
			}

			if (!CheckBoundary(x, y, isMapExpanded, currentMapIdx) || programMap[x, y].BuildingType == BuildingType.None)
			{
				buttonRect.gameObject.SetActive(false);
				tileRect.gameObject.SetActive(false);
				return BuildingType.None;
			}
			else
			{
				buttonRect.transform.position = new Vector3(x, y, Constants.HUMAN_POS_Z);
				buttonRect.sprite = programMap[x, y].BuildingSprite;
				buttonRect.gameObject.SetActive(true);

				if (programMap[x, y].BuildingType == BuildingType.Jump
				|| programMap[x, y].BuildingType == BuildingType.RotateButton
				|| programMap[x, y].BuildingType == BuildingType.ToggleButton)
				{
					tileRect.transform.position = new Vector3(programMap[x, y].ButtonInfo.linkedGridPos.x,
						programMap[x, y].ButtonInfo.linkedGridPos.y,
						Constants.HUMAN_POS_Z);
					tileRect.gameObject.SetActive(true);
				}
				else
				{
					tileRect.gameObject.SetActive(false);
				}
				return programMap[x, y].BuildingType;
			}
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
		private bool IsAbleToLink(Vector2Int start, Vector2Int end)
		{

			int quad1 = GetMapIdxFromPos(start.x, start.y, IsMapExpanded);
			int quad2 = GetMapIdxFromPos(end.x, end.y, IsMapExpanded);

			if (programMap[circuitingButtonPos.x, circuitingButtonPos.y].BuildingType != BuildingType.Jump)
				return quad1 == quad2;

			if (quad1 < 0 || quad2 < 0) return false;

			for (int i = 0; i < linkableIndices.GetLength(0); i++)
			{
				if (quad1 == linkableIndices[i, 0] && quad2 == linkableIndices[i, 1]) return true;
			}

			return false;
		}

		public void OnClickMapGridInNoneMode(int x, int y, bool isSet)
		{
			if (!CheckBoundary(x, y, isMapExpanded, currentMapIdx))
			{
				isCircuiting = false;
				return;
			}

			if (isCircuiting == isSet) return;

			if (isCircuiting)   // 회로작업 중이면 -> 클릭했을 때 전에 클릭했던 버튼과 연결 
			{
				if (circuitingButtonPos.x == x && circuitingButtonPos.y == y) return;

				if (IsAbleToLink(circuitingButtonPos, new Vector2Int(x, y)))
				{
					programMap[circuitingButtonPos.x, circuitingButtonPos.y].ButtonInfo.linkedGridPos
						= new Vector2Int(x, y);
				}
				else
				{
					// TODO - Link가 불가능할 때 실행하는 부분
				}
				isCircuiting = false;
			}
			else
			{
				if (programMap[x, y].BuildingType != BuildingType.Jump
					&& programMap[x, y].BuildingType != BuildingType.ToggleButton
					&& programMap[x, y].BuildingType != BuildingType.RotateButton) return;

				isCircuiting = true;
				circuitingButtonPos = new Vector2Int(x, y);
			}
		}

		public void OnRightClickMapGridInNoneMode(int x, int y)
		{
			if (isCircuiting)       // circuiting 중이면 취소
			{
				isCircuiting = false;
				return;
			}

			if (!CheckBoundary(x, y, isMapExpanded, currentMapIdx)) return;

			if (programMap[x, y].BuildingType == BuildingType.ToggleButton) return;

			switch (programMap[x, y].BuildingType)
			{
				case BuildingType.RotateButton:
					programMap[x, y].OnButtonRotate();
					break;
				case BuildingType.Add1:
				case BuildingType.Sub1:
				case BuildingType.Jump:
				case BuildingType.Double:
				case BuildingType.Button:
					programMap[x, y].ToggleActive(false);
					break;
				case BuildingType.ToggleButton:
					break;
			}
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
				if (CheckBoundary(prevHoverPos.x, prevHoverPos.y, isMapExpanded, currentMapIdx))
					programMap[prevHoverPos.x, prevHoverPos.y].UnpreviewBuilding();
				prevHoverPos.Set(x, y);
				return;
			}
			if (prevHoverPos.x == x && prevHoverPos.y == y) return;

			if (CheckBoundary(prevHoverPos.x, prevHoverPos.y, isMapExpanded, currentMapIdx))
				programMap[prevHoverPos.x, prevHoverPos.y].UnpreviewBuilding();
			if (programMap[x, y].BuildingType == BuildingType.None)
				programMap[x, y].PreviewBuilding(type);
			prevHoverPos.Set(x, y);

		}
		public void OnClickMapGridInBuildingMode(int x, int y, BuildingType type)
		{
			if (!CheckBoundary(x, y, isMapExpanded, currentMapIdx)) return;
			programMap[x, y].SetBuilding(type);
			Managers.Input.OnInputModeChanged(InputMode.None);
		}
		public void OnRightClickMapGridInBuildingMode(int x, int y)
		{
			if (!CheckBoundary(x, y, isMapExpanded, currentMapIdx)) return;
			programMap[x, y].SetBuilding(BuildingType.None);
			programMap[x, y].ButtonInfo = new ButtonInfos(new Vector2Int(x, y));
		}


		public void OnInputModeChanged(InputMode mode)
		{
			if (CheckBoundary(prevHoverPos.x, prevHoverPos.y, isMapExpanded, currentMapIdx))
				programMap[prevHoverPos.x, prevHoverPos.y].UnpreviewBuilding();
			buttonRect.gameObject.SetActive(false);
			tileRect.gameObject.SetActive(false);
			prevDirPad.Set(-1, -1);
			ChangeMapVisibility(mode);
		}
	}
}