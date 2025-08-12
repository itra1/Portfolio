using System.Collections;
using UnityEngine;
using BestHTTP.SocketIO;
using System;
using it.Mobile;
using Leguar.TotalJSON;
using it.Network.Socket;
using System.Collections.Generic;

public class BestSocketIO : SocketClient
{
	private SocketManager _manager;
	private bool _isConnected;

	public override void Disconnect()
	{
		if (_manager != null)
		{
			it.Logger.Log("[WS] Forse disconnect");
			// Leaving this sample, close the socket
			_manager.Close();
			_isConnected = false;
			_manager.Socket.Off();
			//_currentSubscribeTables.Clear();
			_manager = null;
		}
	}

	

	private void Start()
	{
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, (h) =>
		{
			if (UserController.User == null)
				Disconnect();
		});
		com.ootii.Messages.MessageDispatcher.AddListener("NEW_SOCKET_SID", (h) =>
		{
			Authorization();
		});
	}

	public override void EnterTableChanel(ulong id, Action<bool> action = null)
	{
		//if (_tableSubscribeList.Contains(id)) return;
		//if (_currentSubscribeTables.Contains(id)) return;

		//if (_currentIdTableChanel != null && _currentIdTableChanel == id)
		//	return;

		if (_currentIdTableChanel != null && _currentIdTableChanel != id
#if !UNITY_STANDALONE
		&& (OpenTableManager.Instance == null || !OpenTableManager.Instance.OpenPanels.ContainsKey((ulong)_currentIdTableChanel))
#endif
		)
			LeaveTableChanel((ulong)_currentIdTableChanel);

		if (!_tableSubscribeList.Contains(id))
			_tableSubscribeList.Add(id);
		_currentIdTableChanel = id;
		//_currentSubscribeTables.Add(id);
		EnterChanel(GetChanelTable(id), (result) =>
		{
			//if (!result)
			//	_currentSubscribeTables.Remove(id);
			action?.Invoke(result);
		});
	}

	public override void Init()
	{
		it.Logger.Log("[WS] Force Init");

		if (_isConnected || (_manager != null && _manager.Socket.IsOpen)) return;

		it.Logger.Log("[WS] Init");
		if (_manager == null)
		{
			var options = new SocketOptions()
			{
				AutoConnect = false,
				Timeout = TimeSpan.FromMinutes(5),
				ConnectWith = BestHTTP.SocketIO.Transports.TransportTypes.Polling
			};
			// "https://srv.garillapoker.com:8766/socket.io/"
			//_manager = new SocketManager(new Uri("https://dev.garillapoker.com:8766/socket.io/"), options);
			_manager = new SocketManager(new Uri(ServerManager.ServerWS), options);
			_manager.Socket.Off();
			//_currentSubscribeTables.Clear();
		}
		_manager.Encoder = new BestHTTPEncoder();
		it.Logger.Log("[WS] Connecting...");

		_manager.Socket.On(SocketIOEventTypes.Unknown, OnConnectionEvent);
		_manager.Socket.On("reconnect", OnConnectionEvent);
		_manager.Socket.On(SocketIOEventTypes.Connect, OnConnectionEvent);

		_manager.Socket.On(SocketIOEventTypes.Disconnect, (s, p, a) =>
		{
			_isConnected = false;
			it.Logger.Log("[WS] Disconnected!");
			_manager.Socket.Off();
			_manager = null;
			Init();
		});

		_manager.Socket.On(SocketIOEventTypes.Event, (s, p, args) =>
		{
			it.Logger.Log(string.Format("[WS] Event: {0}", p.Payload));
			//ParseEvent(p.Payload, chanel);
		});

		_manager.Socket.On(SocketIOEventTypes.Error, (s, p, args) =>
		{
			it.Logger.LogError(string.Format("[WS] Error: {0}", args[0].ToString()));
		});
		_isConnected = true;
		_manager.Open();

	}

	private void OnConnectionEvent(Socket s, Packet p, object[] a)
	{
		Authorization();
	}

	private void Authorization(){

		it.Logger.Log("[WS] Connected!");

		//string data = it.Helpers.ParserHelper.Seriarizable(new Token(it.Managers.NetworkManager.Token)).CreateString()/*.Replace("\"","\\\"")*/;
		var data = Newtonsoft.Json.JsonConvert.SerializeObject(new Token(it.Managers.NetworkManager.Token));

		Send("authorize", data, (s, p, a) =>
		{

			it.Logger.Log("[WS] authorize!");
			//_currentSubscribeTables.Clear();
			_manager.Socket.Off();
			EnterUserChanel();
			EnterChanel("echo", (result) => { });
			SubscribeExistsTables();
			EmitSocketOpen();
		});
	}

	public void Send(string eventString, object send, BestHTTP.SocketIO.Events.SocketIOAckCallback callback = null)
	{
		if (_manager != null && _manager.Socket.IsOpen)
		{
			it.Logger.Log($"[WS] send {send}");
			_manager.Socket.Emit(eventString, callback, send);
		}
	}

	public override void LeaveTableChanel(ulong id, Action<bool> action = null)
	{
		if (TableManager.Instance.CheckActiveTableId(id))
			return;

		LeaveChanel(GetChanelTable(id), action);


		if (_currentIdTableChanel == id)
			_currentIdTableChanel = null;

		//_currentSubscribeTables.Remove(id);
		_tableSubscribeList.Remove(id);
	}

	private void SubscribeExistsTables()
	{
		_tableSubscribeList.ForEach(x =>
		{
			//if (!_currentSubscribeTables.Contains(x))
			EnterChanel(GetChanelTable(x), null);
		});
	}

	void OnDestroy()
	{
		Disconnect();
	}
	private void EnterChanel(string chanel, Action<bool> action = null)
	{
		//string data = it.Helpers.ParserHelper.Seriarizable(new Channel(chanel)).CreateString();
		var data = Newtonsoft.Json.JsonConvert.SerializeObject(new Channel(chanel));

		it.Logger.Log($"[WS] request EnterChanel {chanel}");
		Send("enterChannel", data, (s, p, d) =>
		{
			it.Logger.Log($"[WS] EnterChanel {chanel} {d[0].ToString()}");
			var value = d[0].ToString().Contains("true");
			if (action != null) action(value);
			if (value)
			{
				//socket.On(chanel, data =>
				//{
				//	ParseEvent(data, chanel);
				//});
				_manager.Socket.On(chanel, (s, p, args) =>
				{
					ParseEvent(p.Payload, chanel);
				});
			}
		});
	}
	private void LeaveChanel(string chanel, Action<bool> action = null)
	{
		Send("leaveChannel", Newtonsoft.Json.JsonConvert.SerializeObject(new Channel(chanel)), null);
		if (_manager != null)
			_manager.Socket.Off(chanel);
	}

	public class IncomingPackage
	{
		public string Event;
	}

	private void ParseEvent(string data, string chanel)
	{
		var dt = Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(data);
		var dt1 = Newtonsoft.Json.JsonConvert.DeserializeObject<IncomingPackage>(dt[1].ToString());

		it.Logger.Log("[WS] socketResponse : " + dt1.Event);
		ProcessPackage(chanel, dt1.Event, dt[1].ToString());
	}


	public void EnterUserChanel(Action<bool> action = null)
	{
		EnterChanel(GetChanelUser(), action);
	}


}