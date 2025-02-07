using HumanFactory.Util;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

		private void Awake()
		{
			Init();
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
					GameplayData data = Serializer.ByteArrayToObject<GameplayData>(buffer);
					Debug.Log(data.ToString());

					string result = await RunSimulation();
					byte[] sendBuff = Encoding.UTF8.GetBytes(result);
					await stream.WriteAsync(sendBuff, 0, sendBuff.Length);
					DebugServer.Log("Transmit: " + result);
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

		private async Task<String> RunSimulation()
		{
			return "Echo";
		}
	}
}