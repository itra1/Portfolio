using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Network.Rest;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LobbyManager : Singleton<LobbyManager>
{

	private void Start()
	{

		PlayerPrefs.SetInt(StringConstants.CloseNow, 0);
#if UNITY_STANDALONE && !UNITY_EDITOR
		//StandaloneController.SetNewWindowName($"Garilla Poker - Main");
		StandaloneController.Instance.SetLobbySize();
#endif

#if !UNITY_ANDROID && !UNITY_WEBGL && !UNITY_IOS
		UiManager.Instance.OpenLobby();
#endif
		if (SplashScreen.Instance != null)
		SplashScreen.Instance.LoadComplete();
	}

}
