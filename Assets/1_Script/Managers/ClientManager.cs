using HumanFactory.Util;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HumanFactory.Manager
{
	/// <summary>
	/// 클라이언트에서 서버로의
	/// TCP 통신에 관한 부분을 다룹니다.
	/// </summary>
	public class ClientManager
	{
		public void Init()
		{

		}

		public void SendMessage()
		{
			_ = Task.Run(() => SendMessageAsync(Serializer.JsonToByteArray(new ClientSimulationData())));
		}

		private async void SendMessageAsync(byte[] buff)
		{
			Debug.Log("SEND MESSAGE");
			TcpClient client = new TcpClient(Constants.IP_ADDR_INHO, Constants.PORT_VM_TCP);
			NetworkStream stream = client.GetStream();

			await stream.WriteAsync(buff, 0, buff.Length);
			await stream.FlushAsync();

			byte[] buffer = new byte[1024];
			int bytesRead = stream.Read(buffer, 0, buffer.Length);
			string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
			Debug.Log("RESPONSE : " + response);

			client.Close();
		}
	}
}