namespace Core.Network.Socket.Consts
{
    public class SocketLogMessage
    {
        public const string ConnectingFormat = "Connecting to socket: {0}";
        public const string DisconnectingFormat = "Disconnecting from socket: {0}";
        public const string ConnectionGoesThroughProxyFormat = "Socket connection goes through proxy: {0}";
        public const string ConnectingRefusedFormat = "Attempt to connect to socket refused. Current state: {0}";
        public const string DisconnectingRefusedFormat = "Attempt to disconnect from socket refused. Current state: {0}";
        public const string Connected = "Socket connected";
        public const string Disconnected = "Socket disconnected";
        public const string ErrorFormat = "Socket error: {0} - {1}";
        public const string Disposed = "Socket disposed";
    }
}