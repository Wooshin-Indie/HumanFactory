using HumanFactory.Util;
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
			Init();
			simulator = GetComponent<Simulator>();
			StartCoroutine(simulator.RunSimulation());
			Task.Run(() => StartServer());
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

				while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
				{
					ClientSimulationData data = Serializer.ByteArrayToObject<ClientSimulationData>(buffer);
					Debug.Log(data.ToString());

					// Simulator에 데이터 넣기
					simulator.PushDatas(data);

					ServerResultData result = new ServerResultData();
					byte[] sendBuff = Serializer.JsonToByteArray(result);
					await stream.WriteAsync(sendBuff, 0, sendBuff.Length);
				}
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

	}
}