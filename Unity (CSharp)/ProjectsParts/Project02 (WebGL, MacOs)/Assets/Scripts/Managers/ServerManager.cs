using System.Collections;
using System.Text;
using System;
using UnityEngine;
using it.Network.Rest;


public class ServerManager : Singleton<ServerManager>
{
	private const string SERVER_DATA = "SERVER_DATA";
	public static event UnityEngine.Events.UnityAction OnServersSetComplete;

	public static bool ExistsServers => !string.IsNullOrEmpty(Server);

	public static string SourceDataBase64 => _sourceBase64;
	public static string Server
	{
		get
		{
			return _server;
		}
	}
	public static string ServerApi
	{
		get
		{
			return Server + "/api/v1";
		}
	}

	public static string ServerWS
	{
		get
		{
			return _serverWS;
		}
	}

	private static string _server;
	private static string _serverWS;
	private static string _sourceBase64;

	private void Start()
	{
		ReadServers();
	}

	private void ReadServers()
	{
		bool isDev = false;
#if SERVER_DEV && UNITY_EDITOR
		isDev = true;
#endif
		//#if UNITY_EDITOR
		//		SetStandartUrls();
#if UNITY_STANDALONE_WIN

		string serverData = CommandLineController.GetServersData();

		if (!string.IsNullOrEmpty(serverData))
		{
			PlayerPrefs.SetString(SERVER_DATA, serverData);
		}
		if (string.IsNullOrEmpty(serverData) && PlayerPrefs.HasKey(SERVER_DATA))
		{
			serverData = PlayerPrefs.GetString(SERVER_DATA);
		}
		if (string.IsNullOrEmpty(serverData))
		{
			return;
		}

		if (isDev /*|| string.IsNullOrEmpty(serverData)*/ || AppConfig.DevServer)
		{
			SetStandartUrls();

			ServersResponse servers = new ServersResponse();
			servers.servers = new Servers();
			servers.servers.game = new System.Collections.Generic.List<string>() { _server };
			_sourceBase64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(servers)));

			it.Logger.Log(_server);
			it.Logger.Log(_sourceBase64);
			return;
		}
		_sourceBase64 = serverData;
		string jServers = Encoding.UTF8.GetString(Convert.FromBase64String(serverData));
		it.Logger.Log(jServers);
		var dataServers = Newtonsoft.Json.JsonConvert.DeserializeObject<it.Network.Rest.ServersResponse>(jServers);

		ConfirmData(dataServers);

#elif UNITY_STANDALONE_OSX
		SetStandartUrls();
		//https://srv.garillapoker.com
		//ConfirmData(new ServersResponse() { servers = new Servers() { game = new System.Collections.Generic.List<string>() { "https://srv.garillapoker.com" } } });
#elif UNITY_WEBGL
		string host = "https://garilla-web.com";
#if !UNITY_EDITOR
		host = WebGLController.GetCurrentURL();
#endif

		if (!string.IsNullOrEmpty(AppConfig.CustomServer) && !string.IsNullOrEmpty(AppConfig.CustomServerWS))
		{
			_server = AppConfig.CustomServer;
			_serverWS = AppConfig.CustomServerWS;
			EmitLoadComplete();
			return;
		}

		if (AppConfig.DevServer || isDev)
		{
			SetStandartUrls();
			return;
		}


		it.Api.UserApi.GetWebGLServers(host, (response) =>
		{
			if (!response.IsSuccess)
			{
				SetStandartUrls();
				return;
			}
			ConfirmData(response.Result);
		});
#elif UNITY_ANDROID
#if UNITY_EDITOR
		SetStandartUrls();
#else
		//https://srv.garillapoker.com
		ConfirmData(new ServersResponse() { servers = new Servers() { game = new System.Collections.Generic.List<string>() { "https://srv.garillapoker.com" } } });
#endif
#elif UNITY_IOS
#if UNITY_EDITOR
		SetStandartUrls();
#else
		//https://srv.garillapoker.com
		ConfirmData(new ServersResponse() { servers = new Servers() { game = new System.Collections.Generic.List<string>() { "https://srv.garillapoker.com" } } });
#endif
#endif

	}

	private void ConfirmData(it.Network.Rest.ServersResponse dataServers)
	{
		if (!string.IsNullOrEmpty(AppConfig.CustomServer) && !string.IsNullOrEmpty(AppConfig.CustomServerWS))
		{
			_server = AppConfig.CustomServer;
			_serverWS = AppConfig.CustomServerWS;
		}
		else
		{
			_server = dataServers.servers.game[0];
			_serverWS = "https://" + _server.Replace("https://", "") + ":8766/socket.io/";
		}
		EmitLoadComplete();
	}

	private void SetStandartUrls()
	{
		if (!string.IsNullOrEmpty(AppConfig.CustomServer) && !string.IsNullOrEmpty(AppConfig.CustomServerWS))
		{
			_server = AppConfig.CustomServer;
			_serverWS = AppConfig.CustomServerWS;
		}
		else
		{
			_server = it.Settings.AppSettings.Servers.Server;
			_serverWS = "https://" + _server.Replace("https://", "") + ":8766/socket.io/";
		}
		//_serverWS = it.Settings.Settings.Servers.Socket;
		EmitLoadComplete();
	}

	private void EmitLoadComplete()
	{
		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.ServersLoaded);
		OnServersSetComplete?.Invoke();
	}

}