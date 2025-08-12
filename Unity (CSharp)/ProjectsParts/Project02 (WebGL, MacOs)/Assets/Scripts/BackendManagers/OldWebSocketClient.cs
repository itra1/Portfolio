/*

using SocketIOClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Network.Rest;
using System.Linq;
using it.Network.Socket;
using it.Settings;

public class OldWebSocketClient : SocketClient
{
	private SocketIO socket;
	public bool IsConnect { get; set; }

	async public override void Disconnect()
	{
		if (socket != null)
		{
			await socket.DisconnectAsync();
			socket = null;
		}
	}

	async void Connect()
	{

		if (socket == null) return;
		await socket.ConnectAsync();

	}

	async public override void Init()
	{
		it.Logger.Log("[WS] Start connect: " + "https://dev.garillapoker.com");
		socket = new SocketIO("https://dev.garillapoker.com", new SocketIOOptions
		{
			Reconnection = true,
			ConnectionTimeout = TimeSpan.FromSeconds(8.0),
			EIO = 3
		});

		socket.OnConnected += async (sender, e) =>
		{
			it.Logger.Log("[WS] OnConnected: " + "https://dev.garillapoker.com");
			IsConnect = true;
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.WSConnect);
			Authorize((result) =>
			{
				_isAuth = result;
				EnterUserChanel();
				EnterChanel("echo", (result) => { });
				if (_currentIdTableChanel != null) EnterTableChanel((ulong)_currentIdTableChanel);

			});
		};

		socket.OnError += async (sender, e) =>
		{
			it.Logger.Log("[WS] OnError: " + e);
			//await socket.ConnectAsync();
		};

		socket.OnDisconnected += async (sender, e) =>
		{
			IsConnect = false;
			it.Logger.Log("OnDisconnected: " + e);
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.WSDisconnect);

			DG.Tweening.DOVirtual.DelayedCall(1, () =>
			{
				socket.ConnectAsync();
			});

		};

		socket.OnPing += async (sender, e) =>
		{
		};

		await socket.ConnectAsync();
	}

	public override void EnterTableChanel(ulong id, Action<bool> action = null)
	{
		if (_currentIdTableChanel != null && _currentIdTableChanel == id) return;

		if (_currentIdTableChanel != null && _currentIdTableChanel != id)
		{
			LeaveTableChanel((ulong)_currentIdTableChanel);
		}

		_currentIdTableChanel = id;
		EnterChanel(GetChanelTable(id), action);
	}

	public override void LeaveTableChanel(ulong id, Action<bool> action = null)
	{
		LeaveChanel(GetChanelTable(id), action);
	}

	public void EnterUserChanel(Action<bool> action = null)
	{
		EnterChanel(GetChanelUser(), action);
	}

	private void Authorize(Action<bool> action)
	{
		Emit("authorize", new Token(it.Managers.NetworkManager.Token), (result) => { action(result.GetValue<bool>()); });
	}


	private void EnterChanel(string chanel, Action<bool> action = null)
	{
		Emit("enterChannel", new Channel(chanel), (result) =>
		{
			var value = result.GetValue<bool>();
			if (action != null) action(value);
			if (value)
			{
				socket.On(chanel, data =>
				{
					ParseEvent(data, chanel);
				});
			}
		});
	}

	private void LeaveChanel(string chanel, Action<bool> action = null)
	{
		Emit("leaveChannel", new Channel(chanel), (result) => { if (action != null) action(result.GetValue<bool>()); });
	}

	async private void Emit(string name, Jsonable data, Action<SocketIOResponse> action)
	{
		if (!IsConnect) return;

		await socket.EmitAsync(name, response =>
		{
			it.Logger.Log($"{name} {response} \n{data}");
			action(response);
		}, data.ToString());
	}

	private void ParseEvent(SocketIOResponse data, string chanel)
	{
		var socketResponse = Leguar.TotalJSON.JSON.ParseString(data.GetValue().GetString());
		var eventName = socketResponse.GetString("event");

		it.Logger.Log("[WS] socketResponse : " + eventName);

		//string eventName = eventSocket.GetString();

		_actionPacket = GetActionPacket(eventName);

		if (_actionPacket == null)
		{
#if UNITY_EDITOR
			it.Logger.LogError(string.Format("����������� ��������� ������ � ����� {0}", eventName));
#endif
			return;
		}

		_actionPacket.SetData(chanel, socketResponse);

		try
		{
			_actionPacket.Parse();
		}
		catch (System.Exception ex)
		{
			it.Logger.LogError("������ �������� ����� ������ " + _actionPacket.GetType().ToString() + " Error: " + ex.Message + " " + ex.StackTrace);
			return;
		}
		_actionPacket.Process();


	}


	private void OnDestroy()
	{
		if (socket != null) socket.DisconnectAsync();
	}

	private void OnApplicationQuit()
	{
		if (socket != null) socket.DisconnectAsync();
	}
}
*/