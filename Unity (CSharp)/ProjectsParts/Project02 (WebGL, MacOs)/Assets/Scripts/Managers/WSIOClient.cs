using System;
using UnityEngine;
#if !UNITY_WEBGL
using BestHTTP.SocketIO;
using BestHTTP.SocketIO.Events;
#endif
using Sett = it.Settings;

namespace it.Managers
{
#if !UNITY_WEBGL
	public class WSIOClient : Singleton<WSIOClient>
	{
		private SocketManager _manager;

		public string Server
		{
			get
			{
				return ServerManager.ServerWS;
			}
		}

		//private void Start()
		//{
		//	Init();
		//}

		public void Init()
		{

			it.Logger.Log("[WS] Connecting " + Server);

			SocketOptions options = new SocketOptions();
			options.AutoConnect = true;
			//options.Auth = (m, s) => new { token = it.Managers.NetworkManager.Token };
			options.ConnectWith = BestHTTP.SocketIO.Transports.TransportTypes.WebSocket;
			options.AdditionalQueryParams = new PlatformSupport.Collections.ObjectModel.ObservableDictionary<string, string>();
			options.Timeout = TimeSpan.FromSeconds(8.0);

			_manager = new SocketManager(new Uri(Server), options);

			SubscribeEvents();
		}

		private void SubscribeEvents()
		{
			_manager.Socket.On(SocketIOEventTypes.Connect, (s, p, a) =>
			{
				it.Logger.Log("[WS] On Connected!");
				com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.SocketOpen, 0.05f);

				Authorize((result) =>
				{
					//isAuth = result;
					//EnterUserChanel();
					//EnterChanel("echo", (result) => { });
					//if (currentIdTableChanel != -1) EnterTableChanel(currentIdTableChanel);

				});

			});
			_manager.Socket.On(SocketIOEventTypes.Disconnect, (s, p, a) =>
			{
				it.Logger.Log("[WS] On Disconnect!");
				com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.SocketClose, 0);
			});
			_manager.Socket.On(SocketIOEventTypes.Error, (s, p, a) =>
			{
				it.Logger.Log("[WS] On Error!");
			});
			_manager.Socket.On(SocketIOEventTypes.Event, (s, p, a) =>
			{
				it.Logger.Log("[WS] On Event!");
			});
			_manager.Socket.On(SocketIOEventTypes.Ack, (s, p, a) =>
			{
				it.Logger.Log("[WS] On Ack!");
			});

		}


		private void Authorize(Action<bool> action)
		{
			Emit("authorize", (s, p, a) =>
			{

				it.Logger.Log(p.Payload);

			},
			 new Token(it.Managers.NetworkManager.Token));
		}

		public void Emit(string eventName, SocketIOAckCallback callback, params object[] args)
		{
			//if (_manager == null) return;
			//if (_manager.State != SocketManager.States.Open) return;

			_manager.Socket.Emit(eventName, callback, args);
		}

		public void Connect()
		{
#if UNITY_EDITOR && DIS_SOCKETS
		return;
#endif
			if (_manager == null) return;
			_manager.Open();
		}
		public void Disconnect()
		{
			if (_manager == null) return;
			_manager.Close();
		}

		[Serializable]
		private class Token : Jsonable
		{
			public Token(string token)
			{
				this.token = token;
			}
			//[JsonPropertyParse]
			public string token;
		}

		[Serializable]
		public class Channel : Jsonable
		{
			public Channel(string channel)
			{
				this.channel = channel;
			}

			//[JsonPropertyParse]
			public string channel;
		}

	}

#endif
}