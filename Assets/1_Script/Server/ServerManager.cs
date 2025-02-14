using HumanFactory.Util;
using MySqlConnector;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace HumanFactory.Server
{
	public class ServerManager : MonoBehaviour
	{
		#region Singleton
		private static ServerManager instance;
		public static ServerManager Instance { get { return instance; } }

		private void Init()
		{
			if (instance == null)
			{
				instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
			else
			{
				Destroy(this.gameObject);
				return;
			}
		}
		#endregion

		private Simulator simulator;
		public Simulator ServerSimulator { get => simulator; }

		private void Awake()
		{
			if (Application.isBatchMode)
			{
				Init();
				simulator = GetComponent<Simulator>();
				Task.Run(() => StartServer());
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private async void StartServer()
		{
			TcpListener listener = new TcpListener(IPAddress.Any, Constants.PORT_VM_TCP);
			listener.Start();
			
			Debug.Log("Server Started!");

			while (true)
			{
				TcpClient client = await listener.AcceptTcpClientAsync();
				DebugServer.Log("ClientConnected");

				_ = Task.Run(() => HandleClient(client));
			}
		}

		private async void HandleClient(TcpClient client)
		{
			try
			{
				byte[] buffer = new byte[1024];
				int bytesRead;
				Stream stream = client.GetStream();

				// Read Simul Data, Simulate
				bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
				ClientSimulationData data = Serializer.ByteArrayToObject<ClientSimulationData>(buffer);
				//simulator.PushDatas(data);

				// Read DB, Response
				byte[] sendBuff = Serializer.JsonToByteArray<ServerResultData>(GetServerResultData());
				await stream.WriteAsync(sendBuff, 0, sendBuff.Length);
			}
			catch(Exception ex)
			{
				DebugServer.Log($"Client Error : {ex.Message}");
			}
			finally
			{
				client.Close();
			}
		}

		private ServerResultData GetServerResultData()
		{
			Debug.Log("RESULT DATA");
			ServerResultData resultData = new ServerResultData();
			resultData.Set();

			using (MySqlConnection conn = new MySqlConnection(Constants.DB_CONN_STR))
			{
				try
				{
					conn.Open();
					string query = "SELECT StageIdx, CycleCount, ButtonCount, KillCount" +
						" FROM results;";

					using (MySqlCommand cmd = new MySqlCommand(query, conn))
					{
						using (MySqlDataReader reader = cmd.ExecuteReader())
						{
							while(reader.Read()){
								int stageIdx = reader.GetInt32("StageIdx");
								int cycleCount = reader.GetInt32("CycleCount");
								int buttonCount = reader.GetInt32("ButtonCount");
								int killCount = reader.GetInt32("KillCount");
								resultData.InsertData(stageIdx, cycleCount, buttonCount, killCount);
							}
						}
					}

					return resultData;
				}
				catch (Exception ex)
				{
					Debug.LogError("Error: " + ex.Message);
				}
			}
			return null;
		}

	}
}