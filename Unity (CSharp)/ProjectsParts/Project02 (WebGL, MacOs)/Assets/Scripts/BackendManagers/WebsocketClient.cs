using System.Collections;
using UnityEngine;
using BestHTTP;
using BestHTTP.WebSocket;
using System;
using it.Settings;
using Leguar.TotalJSON;
using it.Helpers;

public class WebSocketClient : SocketClient
{
	private WebSocket _webSocket;
	public bool IsConnect { get; set; }

	public override void Disconnect()
	{
		_webSocket.Close();
		_webSocket = null;
	}

	public override void EnterTableChanel(ulong id, Action<bool> action = null)
	{
		if (_currentIdTableChanel != null && _currentIdTableChanel == id) return;

		if (_currentIdTableChanel != null && _currentIdTableChanel != id)
		{
			LeaveTableChanel((ulong)_currentIdTableChanel);
		}

		_currentIdTableChanel = id;
		EnterChanel(GetChanelTable(id));
		action.Invoke(true);
	}

	public override void Init()
	{
		//_webSocket = new WebSocket(new Uri(Settings.Servers.Socket));
		_webSocket = new WebSocket(new Uri("https://dev.garillapoker.com/"));

		_webSocket.OnOpen += OnOpen;
		_webSocket.OnMessage += OnMessageReceived;
		_webSocket.OnClosed += OnClosed;
		_webSocket.OnError += OnError;
		_webSocket.Open();
		it.Logger.Log(string.Format("[WS] Connecting to {0}...", new Uri(ServerManager.ServerWS)));
	}
	void OnOpen(WebSocket ws)
	{
		it.Logger.Log("[WS] Open");
		OnSend("authorize", new Token(it.Managers.NetworkManager.Token));
		StartCoroutine(DelayRun());

	}
	void OnMessageReceived(WebSocket ws, string message)
	{
		it.Logger.Log("[WS] message: " + message);
	}
	void OnClosed(WebSocket ws, UInt16 code, string message)
	{
		it.Logger.Log(string.Format("[WS] WebSocket closed! Code: {0} Message: {1}", code, message));

		_webSocket = null;
	}
	void OnError(WebSocket ws, string error)
	{
		it.Logger.Log(string.Format("[WS] An error occured: <color=red>{0}</color>", error));
		_webSocket = null;
	}

	public void OnSend(string packageName, string message)
	{
		JArray outPackage = new JArray();
		outPackage.Add(packageName);
		outPackage.Add(message);

		string sendData = outPackage.CreateString();

		it.Logger.Log(string.Format("[WS] Send: {0}", sendData));

		_webSocket.Send(sendData);
	}
	public void OnSend(string packageName, object message)
	{
		JArray outPackage = new JArray();
		outPackage.Add(packageName);
		outPackage.Add(message);

		_webSocket.Send(outPackage.CreateString());
	}

	public override void LeaveTableChanel(ulong id, Action<bool> action = null)
	{
		LeaveChanel(GetChanelTable(id));
	}
	IEnumerator DelayRun()
	{
		yield return new WaitForSeconds(3);
		EnterChanel("echo");
		EnterUserChanel();
	}

	public void EnterUserChanel()
	{
		EnterChanel(GetChanelUser());
	}
	private void EnterChanel(string chanel)
	{
		OnSend("enterChannel", new Channel(chanel));
	}
	private void LeaveChanel(string chanel)
	{
		OnSend("leaveChannel", new Channel(chanel) );
	}

}