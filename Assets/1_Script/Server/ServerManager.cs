using HumanFactory.Util;
using System;
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

		private void StartServer()
		{
			TcpListener listener = new TcpListener(IPAddress.Any, Constants.PORT_VM_TCP);
			listener.Start();

			Task.Run(() => StartIterative(listener));		
		}

		private async void StartIterative(TcpListener listener)
		{
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
				using (NetworkStream stream = client.GetStream())
				{
					byte[] buffer = new byte[1024];
					int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
					string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
					DebugServer.Log("Received: " + data);

					RunSimulation();
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

		private void RunSimulation()
		{

		}
	}
}