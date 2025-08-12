namespace Core.Network.Socket
{
    public interface ISocketOptions
    {
        string Server { get; }
        string ServerToken { get; }
        string Proxy { get; }
        bool UseProxy { get; }
    }
}