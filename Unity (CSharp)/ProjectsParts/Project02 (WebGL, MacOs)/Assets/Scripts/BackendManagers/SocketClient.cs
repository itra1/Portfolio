using it.Network.Socket;

using JetBrains.Annotations;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public abstract class SocketClient : Singleton<SocketClient>
{
	private Queue<it.Network.Socket.SocketIn> _inPackets = new Queue<it.Network.Socket.SocketIn>();
	public static Dictionary<string, Type> ActionPackets = new Dictionary<string, Type>();
	public List<ulong> _tableSubscribeList = new List<ulong>();
	//public List<ulong> _currentSubscribeTables = new List<ulong>();
	private bool _enterUserchannel;

	public static void CreateInstance<T>() where T : SocketClient
	{
		if (_instance) return;

		_instance = (T)FindObjectOfType(typeof(T), true);

		if (_instance == null)
		{
			var go = new GameObject("SocketClient");
			var t = go.AddComponent<T>();
		}
	}
	protected bool _isAuth = false;
	protected ulong? _currentIdTableChanel = null;
	//protected SocketIn _actionPacket;

	public abstract void Init();
	public abstract void LeaveTableChanel(ulong id, Action<bool> action = null);
	public abstract void EnterTableChanel(ulong id, Action<bool> action = null);
	public abstract void Disconnect();
	public static string GetChanelTable(ulong table_id) { return $"table_{table_id}"; }
	public static string GetChanelUser() { return $"user_{UserController.User.id}"; }

	protected virtual void Awake()
	{
		ActionPackets = FindActionPackets();
		DontDestroyOnLoad(gameObject);
	}
	private void OnEnable()
	{
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserLogin, UserLogin);
	}
	private void UserLogin(com.ootii.Messages.IMessage handle)
	{
		if (UserController.User == null)
		{
			Disconnect();
		}
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

	protected SocketIn GetActionPacket(string action)
	{
		if (!ActionPackets.ContainsKey(action))
			return null;
		return (SocketIn)System.Activator.CreateInstance(ActionPackets[action]);
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

	}

	protected void EmitSocketOpen()
	{

		com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.SocketOpen, 0.01f);
	}

	[Serializable]
	protected class Token : Jsonable
	{
		public Token(string token)
		{
			this.token = token;
		}

		public string token;
	}

	[Serializable]
	public class Channel : Jsonable
	{
		public Channel(string channel)
		{
			this.channel = channel;
		}

		public string channel;
	}

	protected void ProcessPackage(string chanel, string eventName, string package)
	{
		SocketIn actionPacket = null;
		try
		{
			actionPacket = GetActionPacket(eventName);

			if (actionPacket == null)
			{
#if UNITY_EDITOR
				it.Logger.LogError(string.Format("не определен тип пакета {0}", eventName));
#endif
				//StartCoroutine(CorDisable(actionPacket));
				return;
			}

			actionPacket.SetData(chanel, package);

			try
			{
				actionPacket.Parse();
			}
			catch (System.Exception ex)
			{
				it.Logger.LogError("[WS] error parse  " + actionPacket.GetType().ToString() + " Error: " + ex.Message + " " + ex.StackTrace);
				StartCoroutine(CorDisable(actionPacket));
				return;
			}
			actionPacket.Process();

		}
		catch (System.Exception ex)
		{
			if (actionPacket != null)
				it.Logger.LogError("[WS] error parse  " + actionPacket.GetType().ToString() + " Error: " + ex.Message + " " + ex.StackTrace);
			StartCoroutine(CorDisable(actionPacket));
			return;
		}
		StartCoroutine(CorDisable(actionPacket));
	}
	IEnumerator CorDisable(SocketIn actionPacket)
	{
		if (actionPacket == null) yield break;
		yield return new WaitForSeconds(actionPacket.PackageLive);

		while(actionPacket.IsLockDispose && Time.timeSinceLevelLoad - actionPacket.TimeCreate < 100)
			yield return new WaitForSeconds(2f);

		actionPacket.Dispose();
		actionPacket = null;
	}

}