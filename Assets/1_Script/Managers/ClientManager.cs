using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace HumanFactory.Manager
{
	/// <summary>
	/// 클라이언트에서 서버로의
	/// TCP 통신에 관한 부분을 다룹니다.
	/// </summary>
	public class ClientManager
	{

		void Init()
		{
			StartClient();
		}

		void StartClient()
		{
			TcpClient client = new TcpClient("", 12345);
			NetworkStream stream = client.GetStream();

			string data = "";
			byte[] buffer = Encoding.UTF8.GetBytes(data);
			stream.Write(buffer, 0, buffer.Length);

			buffer = new byte[1024];
			int bytesRead = stream.Read(buffer, 0, buffer.Length);
			string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
			Debug.Log("RESPONSE : " + response);

			client.Close();
		}
	}
}