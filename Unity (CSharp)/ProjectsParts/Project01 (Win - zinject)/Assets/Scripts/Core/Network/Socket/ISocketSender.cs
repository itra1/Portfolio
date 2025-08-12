namespace Core.Network.Socket
{
	public interface ISocketSender
	{
		void Send(string eventName, object data);
	}
}