using HumanFactory.Manager;
using MySqlConnector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanFactory.Server
{
	public class Simulator : MonoBehaviour
	{
		private Queue<ClientSimulationData> dataQueue = new Queue<ClientSimulationData>();

		// HACK - 데이터 많이들어오면 Stack으로 감당안될수도 있음
		// 일정 크기 이상이면 json에 저장해두고 Stack 비우는 로직 필요
		// Stack이 비면 Json에서 다시 받아오는 식으로
		public void PushDatas(ClientSimulationData data)
		{
			dataQueue.Enqueue(data);

			if (!isSimulEnd)
			{
				StartCoroutine(RunSimulation());
			}
		}

		private bool isSimulEnd = true;
		public IEnumerator RunSimulation()
		{
			while (true)
			{
				if (dataQueue.Count <= 0) yield break;

				yield return new WaitForSeconds(2.0f);

				isSimulEnd = false;

				// HACK - temp datas
				ClientSimulationData data = new ClientSimulationData();
				data.stageIdx = 0;
				data.saveData = Managers.Data.GetGridDatas(0, 0);

				// Load map and simulate
				MapManager.Instance.LoadStage(data.stageIdx, data.saveData);
				MapManager.Instance.AddPersonWith1x(0f);
				GameManagerEx.Instance.SetExeType(ExecuteType.Play);

				// Wait until simuation ended
				yield return new WaitUntil(() => isSimulEnd);
			}
		}

		public void OnSimulationEnd(GameResultInfo info, bool isSuccess)
		{
			SimulationResult result = new SimulationResult(0, info);
			isSimulEnd = true;

			if (isSuccess)
				InsertResultData(result);
		}



		public void InsertResultData(SimulationResult result)
		{
			using (MySqlConnection conn = new MySqlConnection(Constants.DB_CONN_STR))
			{
				try
				{
					conn.Open();

					string query = @"
                    INSERT INTO results (UserId, StageIdx, CycleCount, ButtonCount, KillCount)
                    VALUES (@UserId, @StageIdx, @CycleCount, @ButtonCount, @KillCount)
                    ON DUPLICATE KEY UPDATE 
                        CycleCount = LEAST(CycleCount, @CycleCount),
                        ButtonCount = LEAST(ButtonCount, @ButtonCount),
                        KillCount = LEAST(KillCount, @KillCount);
                ";

					using (MySqlCommand cmd = new MySqlCommand(query, conn))
					{
						// 파라미터 설정
						cmd.Parameters.AddWithValue("@UserId", result.userId);
						cmd.Parameters.AddWithValue("@StageIdx", result.stageIdx); 
						cmd.Parameters.AddWithValue("@CycleCount", result.cycleCount);
						cmd.Parameters.AddWithValue("@ButtonCount", result.buttonCount);
						cmd.Parameters.AddWithValue("@KillCount", result.killCount);

						cmd.ExecuteNonQuery();
						Debug.Log("Test data inserted successfully.");
					}
				}
				catch (MySqlException ex)
				{
					Debug.LogError("Error: " + ex.Message);
				}
			}
		}
	}
}