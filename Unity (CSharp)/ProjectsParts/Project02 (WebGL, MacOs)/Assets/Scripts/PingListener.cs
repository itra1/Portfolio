using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Settings;
using System;

#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
using System.Net.NetworkInformation;
using System.Threading.Tasks;
#endif
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

namespace Garilla.Ping
{

	public interface IPingListener
	{
		string Host { get; set; }
		void SetPing(long value);
	}

	public class PingListener : MonoBehaviour
	{
		private static List<IPingListener> _listeners = new List<IPingListener>();

		public static PingListener _instance;

		private static int delayCall { get; } = 5;

		private static void Init()
		{
			GameObject go = new GameObject("PingListener");
			_instance = go.AddComponent<PingListener>();
		}

		public static void AddListener(IPingListener listener)
		{
			if (_instance == null)
				Init();

			if (!_listeners.Contains(listener))
				_listeners.Add(listener);

			_instance.GetPing(listener);
		}

		public static void RemoveListeners(IPingListener listener)
		{
			if (_instance == null)
				Init();

			_listeners.Remove(listener);
		}

		//private void Update()
		//{
		//	foreach (var elem in _listeners)
		//		if ((DateTime.Now - elem.LastRequest).TotalSeconds > 20)
		//			GetPing(elem);
		//}


#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
		public async void GetPing(IPingListener listener)
		{
			if (listener == null) return;
			await Task.Delay(delayCall * 10);
			var ping = new System.Net.NetworkInformation.Ping();
			var result = await ping.SendPingAsync(listener.Host.Replace("https://",""));

			if (listener == null)
			{
				ping.Dispose();
				return;
			}

			listener.SetPing(result.Status == System.Net.NetworkInformation.IPStatus.Success ? result.RoundtripTime : 999);
			ping.Dispose();

			await Task.Delay(delayCall * 1000);
			GetPing(listener);
		}
#endif

#if UNITY_WEBGL

		public void Receive(string value)
		{
			var result = value.Split("|");
			var elem = _listeners.Find(x => x.Host == result[0]);

			if (elem != null)
			{
				elem.SetPing(long.Parse(result[1]));
				StartCoroutine(GetPingCor(elem));
			}
		}

		private IEnumerator GetPingCor(IPingListener listener)
		{
			yield return new WaitForSeconds(delayCall);
			GetPing(listener);
		}

		public void GetPing(IPingListener listener)
		{
			if (listener == null) return;

#if !UNITY_EDITOR
			ping(listener.Host);
#endif
		}

		[DllImport("__Internal")]
		private static extern void ping(string address);
#endif

	}
}