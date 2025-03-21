using HumanFactory.Controller;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using HumanFactory.Util;
using HumanFactory.Buttons;
using DG.Tweening;

namespace HumanFactory.Manager
{
    /// <summary>
    /// MapManager의 싸이클 관련 로직들을 담는 partial 클래스
    /// </summary>
    public partial class MapManager
    {

		private List<bool> flags = new List<bool> { false, false, false };
		private List<float> timeSections = new List<float> { 0.3f, 0.5f, 0.7f };
		private List<Func<bool>> secFuncs = new List<Func<bool>>();
		private float cycleElapsedTime = 0f;

		public Action<float> OnCycleTimeChanged { get; set; }

		/** Result Variables **/
		int cycleCount = 0;
		int killCount = 0;

		public float CycleTime
		{
			get => cycleTime;
			set
			{
				OnCycleTimeChanged?.Invoke(value);
				gunnersManagement.SetCycleTime(value);
				outFlag.GetComponent<Animator>().speed = 1 / value;
                foreach (var controller in humanControllers)
				{
					controller.SetAnimSpeed(value);
				}
				cycleTime = value;
			}
		}

		private IEnumerator ProgramCycleCoroutine()
		{
			cycleCount++;
			InitPerCycle();
			while (cycleElapsedTime < CycleTime)
			{
				ExecutePerFrame(cycleElapsedTime, CycleTime);
				yield return null;
				cycleElapsedTime += Time.deltaTime;
			}

			for (int i = 0; i < flags.Count; i++)
			{
				if (!flags[i]) flags[i] = secFuncs[i].Invoke();
			}

			FinPerCycle();
		}


		private bool isPersonAdd = false;
		public bool IsPersonAdd { get => isPersonAdd; }


		public void DoubleCycleTime()
		{
			float prev = CycleTime;
			CycleTime = 0.5f;

			cycleElapsedTime = cycleElapsedTime * (CycleTime / prev);


		}
		public void AddPersonWith1x(float cycleTime = 1f)
		{
			float prev = CycleTime;
			CycleTime = cycleTime;
			cycleElapsedTime = cycleElapsedTime * (CycleTime / prev);

			if (idxIn == 0)
			{
				SaveStage();
				cycleCount = killCount = 0;
				isPersonAdd = true;
			}
		}
		public void AddPersonWithOneCycling()
		{
			float prev = CycleTime;
			CycleTime = 0.3f;
			cycleElapsedTime = cycleElapsedTime * (CycleTime / prev);

			if (idxIn == 0)
			{
				SaveStage();
				isPersonAdd = true;
			}
		}
		public void AddPerson()
		{
			isPersonAdd = true;
		}

		/// <summary>
		/// 싸이클 시작할 떄 수행해야하는 애들
		/// 변수 값 초기화가 주 목적
		/// </summary>
		private int idxIn = 0;
		public int IdxIn { get => idxIn; set => idxIn = value; }
		private void InitPerCycle()
		{
			isCycleRunning = true;
			InitNewPerson();
			InitTeleport();

			cycleElapsedTime = 0f;
			for (int i = 0; i < flags.Count; i++)
			{
				flags[i] = false;
			}

			foreach (HumanController controllers in humanControllers)
			{
				controllers.OnInitPerCycle();
			}
		}


		/// <summary>
		/// 매 프레임 실행하는 함수
		/// </summary>
		private void ExecutePerFrame(float elapsedTime, float maxTime)
		{
			for (int i = 0; i < flags.Count; i++)
			{
				if (flags[i] || !(cycleElapsedTime > CycleTime * timeSections[i])) continue;
				flags[i] = secFuncs[i].Invoke();
			}

			foreach (var controller in humanControllers)
			{
				controller.SetPositionByRatio(elapsedTime / maxTime);
			}
		}

		/// <summary>
		/// 1/3 경과 시, Button을 전부 Release하여 맵을 Original로 만듦
		/// TDOO - Target이 안바뀌는 애들은 Release 할 필요 없음
		/// </summary>
		private bool ExecuteAtOneThird()
		{
			foreach (HumanController controller in humanControllers)
			{
				if (controller.TargetPos == controller.PrevPos) continue;
				if (!CheckBoundary(controller.CurrentPos.x, controller.CurrentPos.y, isMapExpanded)) continue;
				programMap[controller.CurrentPos.x, controller.CurrentPos.y].OnReleased();
			}

			return true;
		}

		// HashSet에 위치를 전부넣어서 TargetPos가 겹치는 애들을 찾음
		private Dictionary<Vector2Int, List<HumanController>> targetPosSet = new Dictionary<Vector2Int, List<HumanController>>();
		private Vector2Int tmpPos;

		/// <summary>
		/// 1/2 경과 시, 플레이어의 Vector2Int CurPos를 업데이트 시킴
		/// CurPos가 겹치는 경우를 확인하고 겹치는 경우에 더할 준비를 해야됨
		/// controller에 변수를 하나두고 체크해서 그 경우엔 곧 사라지도록 or 더해지도록
		/// </summary>
		private bool ExecuteAtHalfTime()
		{
			targetPosSet.Clear();
			return true;
		}


		/// <summary>
		/// 2/3 경과 시, 이동한 위치의 Button을 Press 하고 변경사항 업데이트
		/// </summary>
		private bool ExecuteAtTwoThirds()
		{
			foreach (var controller in humanControllers)
			{
				if (!CheckBoundary(controller.TargetPos.x, controller.TargetPos.y, isMapExpanded)) continue;
				ButtonBase buttonBase = programMap[controller.TargetPos.x, controller.TargetPos.y].ButtonBase;
				buttonBase?.OnPressed(true);
			}
			return true;
		}

		private int idxOut = 0;
		public int IdxOut { get => idxOut; set => idxOut = value; }
		private bool isOutputCorrect = true;
		/// <summary>
		/// 전부 경과하여 이동 완료
		/// 1/2 에서 controller에 체크한 걸로 더하기 연산 먼저 수행
		/// 다음 이동할 위치 설정, 연산 수행(+1, +/- ...)
		/// </summary>
		private void FinPerCycle()
		{
			CalcDuplicatedPos();

			foreach (var controller in humanControllers)
			{
				controller.ExecuteOperand();
			}
			humanControllers.RemoveAll(item => item.OperandType == HumanOperandType.Operand2);

			DoButtonExecution();

			for (int i = humanControllers.Count - 1; i >= 0; i--)
			{
				humanControllers[i].OnFinPerCycle();
				CheckOutOfRange(i);
			}

			CheckIsStageEnded();

			isCycleRunning = false;

			if (isOneCycling && GameManagerEx.Instance.ExeType != ExecuteType.None)
			{
				GameManagerEx.Instance.SetExeType(ExecuteType.Pause); // 게임 정지
				CycleTime = 1f;
			}
		}

        private void CheckOutOfRange(int idx)
		{
			if (!(humanControllers[idx].CurrentPos.x == exitPos.x && humanControllers[idx].CurrentPos.y == exitPos.y))		// 출구가 아닌 경우
			{
				if (!CheckBoundary(humanControllers[idx].CurrentPos.x, humanControllers[idx].CurrentPos.y, isMapExpanded))	// 맵 밖으로 나간경우
				{
					int mapIdx = GetMapIdxFromPos(humanControllers[idx].PrevPos.x, humanControllers[idx].PrevPos.y, isMapExpanded);
					gunnersManagement.DetectEscaped(humanControllers[idx].CurrentPos - humanControllers[idx].PrevPos, mapIdx, isMapExpanded);
					humanControllers[idx].HumanDyingProcessWithoutBox();
					humanControllers.Remove(humanControllers[idx]);
					killCount++;
					return;
				}
				else if (humanControllers[idx].HumanNum < 0 || humanControllers[idx].HumanNum >= 1000)
				{
					int mapIdx = GetMapIdxFromPos(humanControllers[idx].PrevPos.x, humanControllers[idx].PrevPos.y, isMapExpanded);
					gunnersManagement.DetectEscaped(humanControllers[idx].CurrentPos - humanControllers[idx].PrevPos, mapIdx, isMapExpanded);
					programMap[humanControllers[idx].CurrentPos.x, humanControllers[idx].CurrentPos.y].OnReleased();
					humanControllers[idx].HumanDyingProcessWithBox();
					humanControllers.Remove(humanControllers[idx]);
					killCount++;
					return;
				}
				return;
			}
			//human이 output지점 (4,5)이 아닌 바운더리 안이면 continue, (4,5)를 제외한 바운더리 바깥이면 총쏴서 없앰

			if (idxOut < currentStageInfo.outputs.Length) // exitPos일 때 실행되는 코드
			{
				if (humanControllers[idx].HumanNum != currentStageInfo.outputs[idxOut])
				{
					isOutputCorrect = false;
				}

				GameManagerEx.Instance.Cameras[(int)GameManagerEx.Instance.CurrentCamType]
					.GetComponent<CameraBase>().CctvUI?.InOut.SetValue(IdxOut, false, humanControllers[idx].HumanNum);
				idxOut++;
			}

			HumanController tmpController = humanControllers[idx];
			StartCoroutine(HumanExitCoroutine(tmpController));
			humanControllers.Remove(humanControllers[idx]);
		}

		private Vector3 exitTo = new Vector3(0f, 3f, 0f);
        private IEnumerator HumanExitCoroutine(HumanController tmpController)
		{
            tmpController.GetComponent<Animator>().TurnState(Constants.ANIM_PARAM_WALK);
            tmpController.SetAnimSpeed(cycleTime);
			
			tmpController.transform.DOMove(tmpController.transform.localPosition + exitTo, 3 * cycleTime);

			yield return new WaitForSeconds(3 * cycleTime);


            Destroy(tmpController.gameObject);
        }

        private void CheckIsStageEnded() // 스테이지 끝났는지 여부 및 정답 체크하는 함수
		{
			if (GameManagerEx.Instance.ExeType == ExecuteType.None) return;

			// 일정 사이클 넘으면 실패처리
			if (Application.isBatchMode && Managers.Resource.GetStageInfo(currentStage).maxCounts[0] < cycleCount)
			{
				OnFailure();
			}

			// BatchMode일 때, 사람들의 전의 상태와 똑같다면 실패로 판단하고 싸이클 끊기
			if (IsSameWithPrevHumanPos())
			{
				if (idxOut == currentStageInfo.outputs.Length && isOutputCorrect)
				{
					killCount += humanControllers.Count;
					OnSuccess();
				}
				else if (Application.isBatchMode)
				{
					OnFailure();
					return;
				}
			}


			if (humanControllers.Count != 0) return;
			if (idxOut == currentStageInfo.outputs.Length && isOutputCorrect)
			{
				OnSuccess();
			}
		}

		private void InitNewPerson()
		{
			if (isPersonAdd && idxIn < currentStageInfo.inputs.Length)
			{
				HumanController tmpController = Instantiate(humanPrefab, new Vector3(0f, -1f, Constants.HUMAN_POS_Z), Quaternion.identity)
					.GetComponent<HumanController>();
				humanControllers.Add(tmpController);
				StartCoroutine(GetComponent<WaitingsManagement>().InputWaitingsCoroutine());
				StartCoroutine(GetComponent<WaitingsManagement>().WaitingsCoroutine());

				tmpController.HumanNum = currentStageInfo.inputs[idxIn];
				GameManagerEx.Instance.Cameras[(int)GameManagerEx.Instance.CurrentCamType]
					.GetComponent<CameraBase>().CctvUI?.InOut.SetValue(idxIn, true);
				idxIn++;
				isPersonAdd = false;
			}
		}

		private void InitTeleport()
		{
			foreach (HumanController controller in humanControllers)
			{
				if (!CheckBoundary(controller.CurrentPos.x, controller.CurrentPos.y, isMapExpanded)) return;
				
				var jumpable = programMap[controller.CurrentPos.x, controller.CurrentPos.y].ButtonBase as IJumpable;

				if (jumpable != null && jumpable.IsAbleToJump(controller.HumanNum))
				{
					controller.OnTeleport(programMap[controller.CurrentPos.x, controller.CurrentPos.y].ButtonBase.buttonInfo.linkedGridPos); ;
				}
			}
		}

		// 더해질 애들 계산
		private void CalcDuplicatedPos()
		{
			// Dictionary 에 같은 좌표로 이동하는 애들끼리 모음
			foreach (var controller in humanControllers)
			{
				controller.UpdateCurpos();
				tmpPos = controller.CurrentPos;
				if (!targetPosSet.ContainsKey(tmpPos))
				{
					targetPosSet[tmpPos] = new List<HumanController>();
				}
				targetPosSet[tmpPos].Add(controller);
			}

			// 같은 곳에 겹치는 애들만 모음
			IEnumerable<List<HumanController>> tmpList = targetPosSet
				.Where(item => item.Value.Count >= 2)
				.Select(item => item.Value);

			foreach (var list in tmpList)
			{
				list[0].SetAsOperand1();
				for (int i = 1; i < list.Count; i++)
				{
					list[0].SetOperands(list[i].SetAsOperand2());
				}
			}

		}

		private void DoButtonExecution()
		{
			int size = humanControllers.Count;

			for (int i = 0; i < size; i++)
			{
				Vector2Int tmpV = humanControllers[i].CurrentPos;
				if (!CheckBoundary(tmpV.x, tmpV.y, isMapExpanded)) continue;

				var nontargetableButton = programMap[tmpV.x, tmpV.y].ButtonBase as NontargetableButton;
				nontargetableButton?.DoButtonExecution(humanControllers[i]);
			}
		}

		public void AddPersonByDouble(HumanController controller, PadType padType)
		{
			HumanController tmpController = Instantiate(humanPrefab, new Vector3(controller.transform.position.x, controller.transform.position.y, Constants.HUMAN_POS_Z), Quaternion.identity)
				.GetComponent<HumanController>();
			tmpController.SetAsDoubled(controller);
			tmpController.UpdateTargetPosWithDoubleButtonDir(padType);
			humanControllers.Add(tmpController);

		}

		private HashSet<Vector3Int> prevHumansPos = new HashSet<Vector3Int>();

		private bool IsSameWithPrevHumanPos()
		{
			HashSet<Vector3Int> currentPosSet = new HashSet<Vector3Int>();

			foreach (var human in humanControllers)
			{
				currentPosSet.Add(new Vector3Int(human.CurrentPos.x, human.CurrentPos.y,
					(int)programMap[human.CurrentPos.x, human.CurrentPos.y].PadType));
			}

			if (prevHumansPos.Count == 0)
			{
				prevHumansPos = new HashSet<Vector3Int>(currentPosSet);
				return false;
			}

			bool isSame = prevHumansPos.SetEquals(currentPosSet);

			prevHumansPos = new HashSet<Vector3Int>(currentPosSet);
			return isSame;
		}

	}
}