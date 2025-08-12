using System;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_WEBGL
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO3.Events;
#endif
using it.Network.Socket;
using System.Linq;
using System.Reflection;
using Sett = it.Settings;


#if UNITY_EDITOR

using UnityEditor;

#endif

namespace it.Managers
{
#if !UNITY_WEBGL
	public class WSManager : Singleton<WSManager>
	{
		public SocketManager _manager;
		private Queue<SocketIn> _inPackets = new Queue<SocketIn>();

		private string _server = "";
		private string _proxy = "";

		private Queue<SystemQueue> _sendQueue = new Queue<SystemQueue>();

		public static Dictionary<string, Type> ActionPacket = new Dictionary<string, Type>();
		public static Dictionary<string, Type> ActionEvents = new Dictionary<string, Type>();

		private bool _isSendFirst = false;

		private static List<string> _disabledPacket = new List<string>();

		private string Token = "";

		private struct SystemQueue
		{
			public string Name;
			public Leguar.TotalJSON.JSON Data;
		}

		public string Server
		{
			get
			{
				return ServerManager.ServerWS;
			}
		}

		private void Awake()
		{
			_disabledPacket.Add("state_update");
			_disabledPacket.Add("capability_update");
			_disabledPacket.Add("rates_update");

			ActionPacket = FindActionPackets();
			ActionEvents = FindActionEvents();
		}
		public static bool IsDisableBacket(string action)
		{
			return _disabledPacket.Contains(action);
		}

		public static Dictionary<string, Type> FindActionPackets()
		{
			Dictionary<string, Type> packs = new Dictionary<string, Type>();

			Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(it.Network.Socket.SocketIn))).ToArray();

			for (int i = 0; i < types.Length; i++)
			{
				object[] ob = types[i].GetCustomAttributes(false);
				for (int x = 0; x < ob.Length; x++)
				{
					if (ob[x].GetType() == typeof(SocketActionAttribute))
						packs.Add((ob[x] as SocketActionAttribute).AliasName, types[i]);
				}
			}
			return packs;
		}

		public static Dictionary<string, Type> FindActionEvents()
		{
			Dictionary<string, Type> packs = new Dictionary<string, Type>();

			Type[] types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsSubclassOf(typeof(Component))).ToArray();

			for (int i = 0; i < types.Length; i++)
			{
				object[] ob = types[i].GetCustomAttributes(false);
				for (int x = 0; x < ob.Length; x++)
				{
					if (ob[x].GetType() == typeof(SocketEventAttribute))
						packs.Add((ob[x] as SocketEventAttribute).Name, types[i]);
				}
			}
			return packs;
		}


		private void Start()
		{
			Init();
		}

		private void Update()
		{
			while (_inPackets.Count > 0)
			{
				SocketIn pak = _inPackets.Dequeue();
				try
				{
					pak.Parse();
				}
				catch (System.Exception ex)
				{
					it.Logger.LogError("Ошибка парсинга сокет пакета " + pak.GetType().ToString() + " Error: " + ex.Message + " " + ex.StackTrace);
					continue;
				}
				pak.Process();
			}

			if (_manager != null && _manager.State == SocketManager.States.Open && _sendQueue.Count > 0)
			{
				while (_sendQueue.Count > 0)
				{
					var itm = _sendQueue.Dequeue();
					_manager.Socket.Emit(itm.Name, itm.Data);
				}
			}
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

		private void SubscribeEvents()
		{
			_manager.Socket.On<ConnectResponse>("connect", OnConnected);
			_manager.Socket.On("disconnect", OnDisconnected);
			//_manager.Socket.On<ErrorPacket>("error", OnError);

			//_manager.Socket.On<RssPacket>("rss", ( pack) => {  pack.Exec();
			//  AddPacketToProcess(pack); });
			//_manager.Socket.On<RatePacket>("rates", (pack) =>	{  pack.Exec();
			//  AddPacketToProcess(pack); });
			//_manager.Socket.On<ActionPacket>("action", (pack) => {
			//	pack.Exec();
			//	AddPacketToProcess(pack);
			//});
			//_manager.Socket.On<State>("state", (pack) => {
			//	pack.Exec();
			//	AddPacketToProcess(pack);
			//});
		}

		private void AddPacketToProcess(SocketIn packet)
		{
			lock (_inPackets)
			{
				_inPackets.Enqueue(packet);
			}
		}

		public void Init()
		{

			it.Logger.Log("[WS] Connecting " + Server);

			//if (UseProxy && !string.IsNullOrEmpty(_proxy))
			//{
			//	it.Logger.Log("[WS] using proxy " + _proxy);
			//	BestHTTP.HTTPManager.Proxy = new BestHTTP.HTTPProxy(new Uri(_proxy), null, true);
			//}

			SocketOptions options = new SocketOptions();

			//options.Auth = (m, s) => new { token = GameManager.ServerToken };
			options.Auth = (m, s) => new { token = Token };
			options.AdditionalQueryParams = new PlatformSupport.Collections.ObjectModel.ObservableDictionary<string, string>();
			//if (_isForceConnect)
			//{
			//	options.AdditionalQueryParams.Add("force", "true");
			//}
			//options.AdditionalQueryParams.Add("path", "/socket.io/v2/");
			options.ConnectWith = BestHTTP.SocketIO3.Transports.TransportTypes.WebSocket;
			options.AutoConnect = false;

			it.Logger.Log("[WS] Connect " + Server);
			//it.Logger.Log("[WS] Params " + options.BuildQueryParams());
			_manager = new SocketManager(new Uri(Server), options);

			SubscribeEvents();
		}

		private void AuthorizationConfirm(com.ootii.Messages.IMessage handle)
		{
		}

	#region Send events

		public void Emit(string eventName, params object[] args)
		{
			//if (_manager == null) return;
			//if (_manager.State != SocketManager.States.Open) return;

			var ev = _manager.Socket.Emit(eventName, args);
		}

		public void SendEmit(string name, object source)
		{
			return;
			if (ActionEvents.Count == 0) return;

			Leguar.TotalJSON.JSON jSon = new Leguar.TotalJSON.JSON();

			jSon = ParsePacket(name, source);

			if (_manager == null || _manager.State != SocketManager.States.Open)
			{
				_sendQueue.Enqueue(new SystemQueue()
				{
					Name = name,
					Data = jSon
				});
			}
			else
				_manager.Socket.Emit(name, jSon);
		}

		Leguar.TotalJSON.JSON ParsePacket(string name, object source)
		{
			Leguar.TotalJSON.JSON resultJSon = new Leguar.TotalJSON.JSON();

			if (source == null) return resultJSon;

			PropertyInfo[] properties = source.GetType().GetProperties();

			for (int i = 0; i < properties.Length; i++)
			{
				var attrs = properties[i].GetCustomAttributes<it.SocketPropertyAttribute>().ToList();

				for (int a = 0; a < attrs.Count; a++)
				{
					if ((attrs[a].EventName == name || string.IsNullOrEmpty(attrs[a].EventName)) && !resultJSon.ContainsKey(attrs[a].Name))
					{
						if (attrs[a].IsSubObject)
							resultJSon.Add(attrs[a].Name, ParsePacket(name, properties[i].GetValue(source)));
						else
							resultJSon.Add(attrs[a].Name, properties[i].GetValue(source));
					}
				}
			}

			FieldInfo[] fields = source.GetType().GetFields();

			for (int i = 0; i < fields.Length; i++)
			{
				var attrs = fields[i].GetCustomAttributes<it.SocketPropertyAttribute>().ToList();

				for (int a = 0; a < attrs.Count; a++)
				{
					if ((attrs[a].EventName == name || string.IsNullOrEmpty(attrs[a].EventName)) && !resultJSon.ContainsKey(attrs[a].Name))
					{
						if (attrs[a].IsSubObject)
							resultJSon.Add(attrs[a].Name, ParsePacket(name, fields[i].GetValue(source)));
						else
							resultJSon.Add(attrs[a].Name, fields[i].GetValue(source));
					}
				}
			}
			return resultJSon;
		}

#if UNITY_EDITOR

		public static Leguar.TotalJSON.JSON ParsePacketForPrint(string name, Type source)
		{
			Leguar.TotalJSON.JSON resultJSon = new Leguar.TotalJSON.JSON();

			if (source == null) return resultJSon;

			PropertyInfo[] properties = source.GetProperties();

			for (int i = 0; i < properties.Length; i++)
			{
				var attrs = properties[i].GetCustomAttributes<it.SocketPropertyAttribute>().ToList();

				for (int a = 0; a < attrs.Count; a++)
				{
					if ((attrs[a].EventName == name || string.IsNullOrEmpty(attrs[a].EventName)) && !resultJSon.ContainsKey(attrs[a].Name))
					{
						if (attrs[a].IsSubObject)
							resultJSon.Add(attrs[a].Name + "/*" + attrs[a].Description + "*/", ParsePacketForPrint(name, properties[i].PropertyType));
						else
							resultJSon.Add(attrs[a].Name + "/*" + attrs[a].Description + "*/", attrs[a].Description);
					}
				}
			}

			FieldInfo[] fields = source.GetFields();

			for (int i = 0; i < fields.Length; i++)
			{
				var attrs = fields[i].GetCustomAttributes<it.SocketPropertyAttribute>().ToList();

				for (int a = 0; a < attrs.Count; a++)
				{
					if ((attrs[a].EventName == name || string.IsNullOrEmpty(attrs[a].EventName)) && !resultJSon.ContainsKey(attrs[a].Name))
					{
						if (attrs[a].IsSubObject)
							resultJSon.Add(attrs[a].Name + "/*" + attrs[a].Description + "*/", ParsePacketForPrint(name, properties[i].PropertyType));
						else
							resultJSon.Add(attrs[a].Name + "/*" + attrs[a].Description + "*/", attrs[a].Description);
					}
				}
			}
			return resultJSon;
		}

		[MenuItem("Tools/Print send items")]
		public static void PrintEventsPackets()
		{
			if (ActionPacket == null || ActionPacket.Count == 0) ;
			ActionEvents = FindActionEvents();

			Leguar.TotalJSON.JSON resultJSon = new Leguar.TotalJSON.JSON();

			foreach (var elem in ActionEvents.Keys)
			{
				string description = "";

				Type t = ActionEvents[elem];

				var ob = t.GetCustomAttributes<SocketEventAttribute>().ToList();
				for (int x = 0; x < ob.Count; x++)
				{
					if (ob[x].GetType() == typeof(SocketEventAttribute) && (ob[x] as SocketEventAttribute).Name == elem)
						description = (ob[x] as SocketEventAttribute).Description;
				}

				resultJSon.Add(elem + "/*" + description + "*/", ParsePacketForPrint(elem, ActionEvents[elem]));
			}

			string result = System.Text.RegularExpressions.Regex.Unescape(resultJSon.CreatePrettyString());
			string res = System.Text.RegularExpressions.Regex.Replace(result, @"([/\*]*[а-яА-Я_ 0-9]{2,}[\*/]*)(.*)([\n\r])", "$2 $1 $3");

			//it.Logger.Log(resultJSon.CreatePrettyString());
			it.Logger.Log(res);
		}
#endif

	#endregion


	#region SocketIO Events

		private void OnConnected(ConnectResponse resp)
		{
			it.Logger.Log("[WS] Connected!");



			// Отправляем состояния
			//(new it.Network.Packs.CapabilitiesMessage()).Send();

			//if (!_isSendFirst)
			//{
			//	_isSendFirst = true;
			//	it.Managers.StateManager.Instance.ForceSend();
			//}

			//_isForceConnect = false;
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.SocketOpen, 0.05f);

			AppManager.Instance.CallOnMainThread((obj) =>
			{
				it.Logger.Log("[WS] Auth");
				it.Network.Packs.Out.Token tt = new Network.Packs.Out.Token();
				tt.token = it.Managers.NetworkManager.Token;
				tt.Send();
			});

		}

		private void OnDisconnected()
		{
			it.Logger.Log("[WS] Disconnected!");
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.SocketClose, 0);
		}
		//private void OnError(ErrorPacket packet)
		//{

		//	if (packet.code == 1008 || packet.code == 4666 || packet.code == 4667)
		//	{
		//		// 1008 Другой пользователь уже подключен
		//		// 4666 Отключил другой пользователь
		//		// 4667 Отключил администратор
		//		if (packet.code == 1008 || packet.code == 4666)
		//			ExistsAnotherConnect = true;
		//		com.ootii.Messages.MessageDispatcher.SendMessage(this, EventsConstants.SocketError, packet.code, 0);

		//		_manager.Close();
		//	}
		//}


		public void Close()
		{
			if (_manager != null && _manager.State != SocketManager.States.Closed)
				_manager.Close();
		}

	#endregion

	}

#endif
}