using System;

namespace Core.Settings.Server
{
    [Serializable]
    public struct ServerData
    {
        public ServerType Type;
        public string Server;
        public string WebSocket;
    }
}