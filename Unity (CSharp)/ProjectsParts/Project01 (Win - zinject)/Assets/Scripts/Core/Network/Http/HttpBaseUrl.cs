using Core.Configs;
using Core.Options;
using Core.Settings;
using Core.Settings.Server;

namespace Core.Network.Http
{
	/// <summary>
	/// Устаревшее название - "RestManager"
	/// </summary>
	public class HttpBaseUrl : IHttpBaseUrl
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
					server = _projectSettings.GetServer(ServerType.Develop).Server;
				}
				else if (!string.IsNullOrEmpty(_options.CustomServer))
				{
					server = _options.CustomServer;
				}
				else
				{
#if UNITY_EDITOR 
					server = _projectSettings.Server;
#else
					server = _config.GetValue(Configs.Consts.ConfigKey.HttpServer);
#endif
				}
				
				return server;
			}
		}
		
		public string ServerApi
		{
			get
			{
#if SERVER_LOCAL
				return Server;
#else
				return $"{Server}/api2";
#endif
			}
		}
		
		public string ServerDoc
		{
			get
			{
#if SERVER_LOCAL
				return Server;
#else
				return _options.IsDevServerEnabled ? "https://cnp.uintel.ru/devapi" : Server;
#endif
			}
		}
		
		public HttpBaseUrl(IConfig config, IApplicationOptions options, IProjectSettings projectSettings)
		{
			_config = config;
			_options = options;
			_projectSettings = projectSettings;
		}
	}
}