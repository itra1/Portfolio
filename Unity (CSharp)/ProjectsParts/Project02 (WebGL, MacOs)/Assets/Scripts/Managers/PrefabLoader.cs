using DarkTonic.MasterAudio;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

public class PrefabLoader : MonoBehaviour
{
	[SerializeField] private AppManager _appManager;
	[SerializeField] private MasterAudio _masterAudio;

	private void Awake()
	{
		//#if UNITY_STANDALONE
		//		SocketClient.CreateInstance<SocketIOClient>();
		//#else
		SocketClient.CreateInstance<BestSocketIO>();
		//#endif
		Instantiate(_appManager.gameObject);
		//#if !UNITY_WEBGL
		if (!AppConfig.DisableAudio)
		{
			var ma = GameObject.Find("MasterAudio");
			if (ma == null)
				Instantiate(_masterAudio.gameObject);
		}
		//#endif
	}

	[ContextMenu("TestReturnString")]
	public void TestReturnString()
	{
		var c = Marshal.PtrToStringAnsi(ReturnText());
		Debug.Log(c);
	}
	[ContextMenu("TestEncrupt")]
	public void TestEncrupt()
	{
		var c = Marshal.PtrToStringAnsi(EncodeString("encrupt text"));
		Debug.Log(c);
	}
	[DllImport("RSA", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr ReturnText();
	[DllImport("RSA", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr EncodeString(string targetString);

}
