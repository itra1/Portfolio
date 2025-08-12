using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Generals.Network.Http;
using Generals.Network.SocketTcp;
using Generals.Network.SocketWeb;

#if UNITY_EDITOR
using UnityEditor;
namespace Generals.Network {
	[CustomEditor(typeof(NetworkManager))]
	public class NetworkManagerEditor : Editor {

		bool stat = false;

		public override void OnInspectorGUI() {

			NetworkManager script = (NetworkManager)target;

			stat = EditorGUILayout.BeginToggleGroup("Statistic", stat);
			if(stat) {
				GUILayout.Label("Statistic");
				GUILayout.Label(string.Format("Ping: {0,15:N0}", script.currentPing));
				GUILayout.Label(string.Format("In: {0,15:N0} byte", script._byteIn));
				GUILayout.Label(string.Format("Out: {0,15:N0} byte", script._byteOut));
				GUILayout.Label(string.Format("All: {0,15:N0} byte", script._byteAll));
			}
			EditorGUILayout.EndHorizontal();
			base.OnInspectorGUI();
		}

	}
}
#endif

namespace Generals.Network {

	/// <summary>
	/// Сетевой менеджер
	/// </summary>
	public class NetworkManager : MonoBehaviour {

		public static NetworkManager instance;                          // Ссылка на экземпляр
		public List<ServerParametrs> serverParametrs;                   // Массив серверов

		public static event Action<string> OnAuthComplited;

		public static NetState networkState;                            // Состояние сети
		public ServerType serverType = ServerType.developer;            // Тип сервера
		public SocketType connectType = SocketType.webSocket;           // Тип соединения

		[HideInInspector]
		public HttpApi apiService;                                     // HTTP соединения
		[HideInInspector]
		public NetTcpSocket netTcpSocket;                              // Контроллер TCP соединения
		[HideInInspector]
		public NetWebSocket netWebSocket;                              // Контроллер WEB соединения

		public const int SOCKET_TIMEOUT=5000;
		private Queue<Packet> in_packets = new Queue<Packet>();

		public bool showPing;
		private const int PING_INTERVAL = 5000;
		private int pingtime = Constants.GTIME;

		#region Статистика

		[HideInInspector]
		public int currentPing = 0;

		[HideInInspector]
		public int _byteIn;
		public int byteIn {
			get { return _byteIn; }
			set {
				byteAll += value - _byteIn;
				_byteIn = value;
			}
		}
		[HideInInspector]
		public int _byteOut;
		public int byteOut {
			get { return _byteOut; }
			set {
				byteAll += value - _byteOut;
				_byteOut = value;
			}
		}
		[HideInInspector]
		public int _byteAll;
		public int byteAll { get { return _byteAll; } set { _byteAll = value; } }

		#endregion

		public Action OnConnect;
		bool OnConnectEvent;
		public Action OnDisconnect;
		bool OnDisconnectEvent;

		public static string token { get { return (instance == null || instance.apiService == null ? "" : instance.apiService.token); } }
		public static string application_name { get { return (instance == null || instance.apiService == null ? "" : instance.apiService.application_name); } }

		public bool isConnect {
			get { return (networkState == NetState.ServerConnected); }
		}

		private void Awake() {
			if(instance != null) {
				Destroy(this);
				return;
			}
			instance = this;
			apiService = GetComponent<HttpApi>();
			networkState = NetState.None;
		}

		private void Start() {

#if UNITY_EDITOR
			AuthorizationHTTP("empty", SocialNetwork.EDITOR);
#endif
		}

		private void OnDestroy() {
			if(instance == this)
				SocketDisconnect();
		}

		private void Update() {

			if(OnConnectEvent) {
				OnConnectEvent = false;
				if(OnConnect != null) OnConnect();
			}

			if(OnDisconnectEvent) {
				OnDisconnectEvent = false;
				if(OnDisconnect != null) OnDisconnect();
			}

			ProcessPackets();
			if(pingChange) PingUpdate();
		}

		#region Авторизация

		public void AuthorizationHTTP(string tocken, SocialNetwork type) {
			NetworkManager.instance.apiService.LoginSocial(tocken, type, User.instance.OnAuthComplited);
		}

		void AuthConfirm(Generals.Network.Http.HttpApi.AuthMessage message) {

			if(message == null) {
				Debug.LogError("Not authed");
				return;
			}
			Debug.Log("Account: " + message.accountId);
			Debug.Log("Token: " + message.token);
			Debug.Log("Profile Id: " + message.profileId);
			Debug.Log("Status Code: " + message.statusCode);

		}

		#endregion

		#region Сокеты

		public void Connect(ServerType server, SocketType socket) {
			serverType = server;
			connectType = socket;
			SocketConnect(connectType);
		}

		public void SocketConnect(SocketType socketType) {
			this.connectType = socketType;

			switch(this.connectType) {
				case SocketType.tcpSocket: {
						netTcpSocket.Init(this, serverParametrs.Find(x => x.server == serverType && x.connect == connectType));
						netTcpSocket.Connect();
						break;
					}
				case SocketType.webSocket: {
						netWebSocket.Init(this, serverParametrs.Find(x => x.server == serverType && x.connect == connectType));
						netWebSocket.Connect();
						break;
					}
			}
		}

		public void SocketDisconnect() {

			if(!isConnect) return;
			networkState = NetState.Disconnected;

			Debug.Log("Disconnecting...");
			switch(this.connectType) {
				case SocketType.tcpSocket: {
						netTcpSocket.Disconnect();
						break;
					}
				case SocketType.webSocket: {
						netWebSocket.Disconnect();
						break;
					}
			}
		}

		public void OnSocketOpen(string sender) {
			networkState = NetState.ServerConnected;
			OnConnectEvent = true;
		}

		public void OnSocketClose(string reason) {
			Debug.Log("Disconnected");
			in_packets.Clear();
			networkState = NetState.Disconnected;
			OnDisconnectEvent = true;
		}

		public void OnSocketReceiveMessage(string message) { }

		public void OnSocketReceiveData(byte[] data) {

			short PacketId = System.BitConverter.ToInt16(data, 0);
			Packet packet = Constants.getPacket(PacketId);

			if(packet == null) {
				Debug.LogWarning("Unknown packet " + PacketId.ToString("X"));
				return;
			}

			try {
				packet.setData(data, 2);
				packet.ReadImpl();
				packet.setData(null, 0);
				lock(in_packets) in_packets.Enqueue(packet);

				return;
			} catch(Exception e) {
				Debug.LogError("Error " + e.Message + " while reading packet " + PacketId.ToString("X") + "\n" + e.StackTrace);
			}
		}

		public void OnSocketError(string error) {
			Debug.LogError("WebSocket Error : " + error);
		}

		/// <summary>
		/// Обработка очереди пакетов
		/// </summary>
		void ProcessPackets() {
			if(!isConnect) return;

			if(in_packets != null) {
				lock(in_packets) {
					while(in_packets.Count > 0) {
						Packet msg = in_packets.Dequeue();
						if(msg != null) { msg.Process(); }
					}

					if((Constants.GTIME - pingtime > PING_INTERVAL)) {
						pingtime = Constants.GTIME;
						SendPacket(new out_Ping());
					}
				}
			}
		}

		/// <summary>
		/// Отправка пакета
		/// </summary>
		/// <param name="packet">Пакет</param>
		/// <returns></returns>
		public static bool SendPacket(INetOutPacket packet) {
			return instance._SendPacket(packet);
		}

		public bool _SendPacket(INetOutPacket packet) {

			if(!isConnect) {
				Debug.LogError("Trying send packet to closed socket!!");
				return false;
			}

			switch(this.connectType) {
				case SocketType.tcpSocket: {
						return netTcpSocket.SendPacket(packet);
					}
				case SocketType.webSocket: {
						return netWebSocket.SendPacket(packet);
					}
			}
			return false;
		}

		#endregion

		#region Пинг

		public static event Action<int> OnPingChange;
		bool pingChange;

		/// <summary>
		/// Пинг
		/// </summary>
		public void Pong() {
			currentPing = Constants.GTIME - pingtime;
			pingChange = true;
		}

		void PingUpdate() {
			if(OnPingChange != null) {
				pingChange = false;
				OnPingChange(currentPing);
			}
		}

		#endregion

		public static WWWForm CreateForm() {
			WWWForm form = new WWWForm();
			form.headers.Add("X-Access-Token", token);
			form.headers.Add("X-Application-Name", application_name);
			return form;
		}
	}
}