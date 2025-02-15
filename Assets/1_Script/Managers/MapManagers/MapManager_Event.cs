using System.Runtime.InteropServices.WindowsRuntime;
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
				tileRect.gameObject.SetActive(true);

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
				|| programMap[x, y].BuildingType == BuildingType.Jump0
				|| programMap[x, y].BuildingType == BuildingType.Rotate
				|| programMap[x, y].BuildingType == BuildingType.Toggle)
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

		// 다른 Grid인지,
		// 해당 linkedPos를 참조하고있는 다른 버튼이 있는지 확인합니다.
		private bool IsAbleToLink(Vector2Int start, Vector2Int end)
		{
			int quad1 = GetMapIdxFromPos(start.x, start.y, IsMapExpanded);
			int quad2 = GetMapIdxFromPos(end.x, end.y, IsMapExpanded);

			if (programMap[circuitingButtonPos.x, circuitingButtonPos.y].BuildingType != BuildingType.Jump 
				&& programMap[circuitingButtonPos.x, circuitingButtonPos.y].BuildingType != BuildingType.Jump0)
			{
				if (programMap[circuitingButtonPos.x, circuitingButtonPos.y].BuildingType == BuildingType.Toggle
						&& IsDestToPosExist(end.x, end.y, BuildingType.Toggle, IsMapExpanded))
					return false;
				if (programMap[circuitingButtonPos.x, circuitingButtonPos.y].BuildingType == BuildingType.Rotate
					&& IsDestToPosExist(end.x, end.y, BuildingType.Rotate, IsMapExpanded))
					return false;

				return quad1 == quad2;
			}

			if (quad1 < 0 || quad2 < 0) return false;


			for (int i = 0; i < linkableIndices.GetLength(0); i++)
			{
				if (quad1 == linkableIndices[i, 0] && quad2 == linkableIndices[i, 1]) return true;
			}

			return false;
		}

		private bool IsDestToPosExist(int x, int y, BuildingType type, bool isExpanded)
		{
			if (isExpanded)
			{
				for (int i = 0; i < 2 * mapSize.x + mapInterval.x; i++)
				{
					for (int j = 0; j < 2 * mapSize.y + mapInterval.y; j++)
					{
						if (programMap[i, j].BuildingType == type 
							&& programMap[i, j].ButtonInfo.linkedGridPos.x == x 
							&& programMap[i, j].ButtonInfo.linkedGridPos.y == y)
						{
							return true;
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < mapSize.x; i++)
				{
					for (int j = 0; j < mapSize.y; j++)
					{
						if (programMap[i, j].BuildingType == type
							&& programMap[i, j].ButtonInfo.linkedGridPos.x == x
							&& programMap[i, j].ButtonInfo.linkedGridPos.y == y)
						{
							return true;
						}
					}
				}
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
					&& programMap[x, y].BuildingType != BuildingType.Jump0
					&& programMap[x, y].BuildingType != BuildingType.Toggle
					&& programMap[x, y].BuildingType != BuildingType.Rotate) return;

				Managers.Sound.PlaySfx(SFXType.UI_Hover, 1.0f, 0.8f);
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

			if (programMap[x, y].BuildingType == BuildingType.Toggle) return;

			switch (programMap[x, y].BuildingType)
			{
				case BuildingType.Rotate:
					programMap[x, y].OnButtonRotate();
					Managers.Sound.PlaySfx(SFXType.UI_Hover, 1.0f, 0.8f);
					break;
				case BuildingType.Add:
				case BuildingType.Sub:
				case BuildingType.Jump:
				case BuildingType.Jump0:
				case BuildingType.Double:
				case BuildingType.NewInput:
					programMap[x, y].ToggleActive(false);
					Managers.Sound.PlaySfx(SFXType.UI_Hover, 1.0f, 0.8f);
					break;
				case BuildingType.Toggle:
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
			Managers.Sound.PlaySfx(SFXType.Button_Put);
			programMap[x, y].SetBuilding(type);
			Managers.Input.OnInputModeChanged(InputMode.None);
		}
		public void OnRightClickMapGridInBuildingMode(int x, int y)
		{
			if (!CheckBoundary(x, y, isMapExpanded, currentMapIdx)) return;
			Managers.Sound.PlaySfx(SFXType.Button_Remove, .8f);
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