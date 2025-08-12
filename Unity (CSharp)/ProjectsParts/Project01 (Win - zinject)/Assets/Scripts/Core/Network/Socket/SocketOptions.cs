using Core.Configs;
using Core.Configs.Consts;
using Core.Options;
using Core.Settings;
using Core.Settings.Server;

namespace Core.Network.Socket
{
    public class SocketOptions : ISocketOptions
    {
        private readonly IConfig _config;
        private readonly IApplicationOptions _options;
        private readonly IProjectSettings _projectSettings;

        public string Server
        {
            get
            {
                string server;
				
                if (_options.IsDevServerEnabled)
                {
                    server = _projectSettings.GetServer(ServerType.Develop).WebSocket;
                }
                else if (!string.IsNullOrEmpty(_options.CustomServer))
                {
                    server = $"{_options.CustomServer}/socket.io/v2/";
                }
                else
                {
#if UNITY_EDITOR
                    server = _projectSettings.WebSocket;
#else
					server = _config.GetValue(ConfigKey.WebSocketServer);
#endif
                }
				
                return server;
            }
        }

        public string ServerToken => _options.ServerToken;
        
        public string Proxy => _config.GetValue(ConfigKey.Proxy);
		
        public bool UseProxy
        {
            get
            {
#if UNITY_EDITOR && !ENABLE_PROXY_EDITOR
                return false;
#else
				return !_options.IsDevServerEnabled;
#endif
            }
        }
        
        public SocketOptions(IConfig config, IApplicationOptions options, IProjectSettings projectSettings)
        {
            _config = config;
            _options = options;
            _projectSettings = projectSettings;
        }
    }
}