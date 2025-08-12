using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(BrowserContact))]
public class BrowserContactEditor: Editor {
	private string useComand = "";

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		useComand = EditorGUILayout.TextArea(useComand);

		if (GUILayout.Button("Send")) {
			((BrowserContact)target).Callback(useComand);
		}

	}
}

#endif

public class BrowserContact : Singleton<BrowserContact> {
	
	// Подписываемся на калбаки
	[DllImport("__Internal")]
	public static extern void Subscribe();
	
	// Изменение раунда
	[DllImport("__Internal")]
	public static extern void BattleRoundChange(string roundId);
	
	// Изменение выбранного плеера
	[DllImport("__Internal")]
	public static extern void BattleSelectPlayer(string enemyId);
	
	// Конец уровня
	[DllImport("__Internal")]
	public static extern void BattleEnd();

	// Установка аттаки
	[DllImport("__Internal")]
	public static extern void BattleSetAttack(string dataAttack);

	// Установка аттаки
	[DllImport("__Internal")]
	public static extern void Auth();
	
	// Установка аттаки
	[DllImport("__Internal")]
	public static extern void GetMapNum();

	// Установка аттаки
	[DllImport("__Internal")]
	public static extern void GetCookie(string param);

	// Установка аттаки
	[DllImport("__Internal")]
	public static extern void NetRequest(int requestId, string type, string url, string body, int isAuth);

	// Установка аттаки
	[DllImport("__Internal")]
	public static extern void ShowBattleLog(string battleId);
	// Установка аттаки
	[DllImport("__Internal")]
	public static extern void OpenMercenaryList();

	private void Start() {
#if UNITY_WEBGL && !UNITY_EDITOR
		Subscribe();
#endif
	}

	public void Callback(string dataText) {
		
		ReceivedData rData = JsonUtility.FromJson<ReceivedData>(dataText);

		switch (rData.name) {
			// Изменение защиты
			case "CORRECT_PROTECT":
				PlayersManager.Instance.ClearProtect();
				break;
			// Установка защиты
			case "SET_PROTECT":
				PlayersManager.Instance.SetProtected(Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<int>>>(rData.param));
				break;
			// Переобмундирование
			case "DRESS_PLAYER":
				PlayersManager.Instance.PlayerChangeDress();
				break;
			default:
				break;
		}

	}

	public void NetCallback(string data) {
		NetworkManager.Instance.AnswerTest(Newtonsoft.Json.JsonConvert.DeserializeObject<NetJsAnswer>(data));
	}

	public void SetMap(int num) {
		BattleManager.Instance.StartLoadMap(num);
	}

	public void SetAuth(string data) {
		NetworkManager.Instance.StartAuth(Newtonsoft.Json.JsonConvert.DeserializeObject<AuthJsData>(data));
		
		//BattleManager.Instance.StartLoadMap(num);
	}

	private Action<string> OnGetCookieCallback;

	public void SetCookie(string dataText) {

		if (OnGetCookieCallback == null) return;

		OnGetCookieCallback(dataText);
		OnGetCookieCallback = null;
	}

	public void SetBlockCookie(string data) {
		NetworkManager.Instance.SetOuterData(Newtonsoft.Json.JsonConvert.DeserializeObject<CookieJsData>(data));
	}
	
	public void OnGetCookie(string dataText, Action<string> callback) {
		OnGetCookieCallback = callback;
#if UNITY_WEBGL && !UNITY_EDITOR
		GetCookie(dataText);
#endif
	}

	public void OnGetMapNum() {
#if UNITY_WEBGL && !UNITY_EDITOR
		GetMapNum();
#endif
	}

	public void OnAuth() {
#if UNITY_WEBGL && !UNITY_EDITOR
		Auth();
#endif
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattleEnd))]
	public void OnBattleEnd(ExEvent.BattleEvents.BattleEnd data) {
#if UNITY_WEBGL && !UNITY_EDITOR
		BattleEnd();
#endif
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.StartBattle))]
	public void OnBattleStart(ExEvent.BattleEvents.StartBattle data) {
#if UNITY_WEBGL && !UNITY_EDITOR
		BattleRoundChange(data.battleStart.battle_info.round.ToString());
#endif
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.BattleEvents.BattleRoundChange))]
	public void OnBattleRoundChange(ExEvent.BattleEvents.BattleRoundChange data) {
#if UNITY_WEBGL && !UNITY_EDITOR
		BattleRoundChange(data.battleRound.ToString());
#endif
	}
	
	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.PlayerSelect))]
	public void OnPlayerSelect(ExEvent.GameEvents.PlayerSelect data) {
#if UNITY_WEBGL && !UNITY_EDITOR
		BattleSelectPlayer(data.player.playerInfo.pid.ToString());
#endif
	}

	public void OnBattleSetAttack(string dataAttack) {
		Debug.Log("Battel set attack = " + dataAttack);
#if UNITY_WEBGL && !UNITY_EDITOR
		BattleSetAttack(dataAttack);
#endif
	}

	public void OnShowBattleLog(string battleId) {
		Debug.Log("Show battle log: " + battleId);
#if UNITY_WEBGL && !UNITY_EDITOR
		ShowBattleLog(battleId);
#endif
	}

	public void OnOpenMercenaryList() {

#if UNITY_WEBGL && !UNITY_EDITOR
		OpenMercenaryList();
#endif
	}

	public void OnNetRequest(int reqId, string type, string url, string body, bool isAuth) {
#if UNITY_WEBGL && !UNITY_EDITOR
		NetRequest(reqId, type, url, body, isAuth ? 1 : 0);
#endif
	}

}

public class CookieJsData {
	public string key;
	public string value;
}

public class AuthJsData {
	public string userName;
	public string password;
}

public class ReceivedData {
	public string name;
	public string param;
}

public class ReceivedProtect {
	public string name;
	public List<List<int?>> param;
}

public class ProtectParam {
	public int?[][] param;
}