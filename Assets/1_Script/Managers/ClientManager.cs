using HumanFactory.Util;
using System.Net.Sockets;
using System.Threading.Tasks;

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
			TcpClient client = new TcpClient(Constants.IP_ADDR, Constants.PORT_VM_TCP);
			NetworkStream stream = client.GetStream();

			await stream.WriteAsync(buff, 0, buff.Length);
			await stream.FlushAsync();

			client.Close();
		}
	}
}