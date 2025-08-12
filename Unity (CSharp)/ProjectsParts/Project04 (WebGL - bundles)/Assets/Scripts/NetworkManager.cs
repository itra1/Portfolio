//#define TEST_REQUEST
using System;
using System.Collections;
using System.Collections.Generic;
using Network.Input;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : Singleton<NetworkManager> {

	[SerializeField] private string _userName; // Имя пользователя

	[SerializeField] private string _userPassword; // Пароль пользователя

	private string _cookie;
	private string _tok;
	
	private int numberRequest = 0;

	public string tok {
		get { return _tok; }
		set { _tok = value; }
	}

	private void Start() {
		StartCoroutine(queuePostWorker());
	}

	public class RequestData {
		public string url;
		public bool isFirst;
		public NetSendData sendData;
		public System.Action<int, string> after;
	}
	List<RequestData> requestQueue = new List<RequestData>();

	private class ResporseReady {
		public int reqId;
		public System.Action<int, string> after;
	}
	private List<ResporseReady> responseList = new List<ResporseReady>();


	IEnumerator PostWithHeaders(string url, NetSendData data, System.Action<int, string> after = null) {

#if UNITY_EDITOR
		Debug.Log("Request: " + url);
#endif

		var customHeaders = GetHeaders();

		if (!String.IsNullOrEmpty(_tok))
			url += "&tok=" + _tok;
		
		WWW www =
#if UNITY_EDITOR
		new WWW(url, (data != null ? data.GetForm().data : null), GetHeaders());
#else
			new WWW(url, (data != null ? data.GetForm().data : null), GetHeaders());
#endif
		yield return www;

		string resp = null;
		resp = www.text;
#if UNITY_EDITOR
		Debug.Log("Request: " + url);
		Debug.Log("Response: " + resp);
#endif

		bool tryAgein = (string.IsNullOrEmpty(resp) && string.IsNullOrEmpty(_cookie));

		foreach (var key in www.responseHeaders.Keys) {
			if (key.ToUpper() == "SET-COOKIE") {
				CutCookie(www.responseHeaders[key]);
				//_cookie = www.responseHeaders[key];
				ParseTok(_cookie);
			}
		}

		if (string.IsNullOrEmpty(resp)) {
			Debug.Log("Нет ответа с сервера");

			if (tryAgein)
				yield return PostWithHeaders(url, data, after);

			yield break;
		}

		int code = string.IsNullOrEmpty(www.error) ? 200 : int.Parse(www.error.Substring(0, 3));
		if (after != null) after(code, resp);

	}
	
	// Установка параметров извне
	public void SetOuterData(CookieJsData data) {

		switch (data.key) {
			case "tok":
				tok = data.value;
				break;
			case "cookie":
				_cookie = data.value;
				break;
		}

	}

	void PostTest(string url, NetSendData form = null, System.Action<int, string> after = null, bool isAuth = false) {

		numberRequest++;

		string body = "";
		if (form != null)
			body = JsonUtility.ToJson(form);

		ResporseReady req = new ResporseReady() {
			reqId = numberRequest,
			after = after
		};
		responseList.Add(req);

		BrowserContact.Instance.OnNetRequest(req.reqId, "POST", url, body, isAuth);

	}

	public void AnswerTest(NetJsAnswer answer) {

		ResporseReady ans = responseList.Find(x => x.reqId == answer.id);

		if (ans == null) return;

		ans.after(int.Parse(answer.code), answer.text);
		responseList.Remove(ans);
	}

	void CutCookie(string cookie) {
		//Debug.Log("Set cookie = " + cookie);
		if (!string.IsNullOrEmpty(_cookie)) return;
		_cookie = cookie;

		var dataArr = cookie.Split(new char[] { ';' });

		for (int i = 0; i < dataArr.Length; i++) {
			var ind = dataArr[i].IndexOf("tok=");

			if (ind != -1) {
				_tok = dataArr[i].Substring(ind + 4);
				dataArr[i] = "tok=" + _tok;
			}
		}
		_cookie = String.Join(";", dataArr);

	}

	private void ParseTok(string data) {
		var dataArr = data.Split(new char[] { ';' });
		foreach (var elem in dataArr) {
			var ind = elem.IndexOf("tok=");

			if (ind != -1) {
				_tok = elem.Substring(ind + 4);
				_cookie = String.Join(";", dataArr);
			}
		}
	}

	public void Authorization(Action OnAuth) {
		this.OnAuth = OnAuth;
		;

#if TEST_REQUEST

		BrowserContact.Instance.OnAuth();

#else

		AuthJsData sendData = new AuthJsData() {
			userName = _userName,
			password = _userPassword
		};
		StartAuth(sendData);
#endif

	}

	private Action OnAuth;

	public void StartAuth(AuthJsData authData) {

		AuthSendData sendData = new AuthSendData() {
			action = "login",
			login = authData.userName,
			password = authData.password
		};


		QueuePost(Parametrs.Instance.apiServer + "/index_parser.php", sendData, (code, resp) => {
			//return;
			if (OnAuth != null) OnAuth();
		}, true);

	}

	public void BattleReady() {
		QueuePost(Parametrs.Instance.apiServer + "/game/battles/battle_test.php", null, (code, resp) => {
			Debug.Log(resp);
		});
	}


	/// <summary>
	/// Заголовки
	/// </summary>
	/// <returns></returns>
	public Dictionary<string, string> GetHeaders() {
		Dictionary<string, string> customHeaders = new Dictionary<string, string>();
		if (!String.IsNullOrEmpty(_cookie))
			customHeaders.Add("Cookie", _cookie);

		return customHeaders;
	}

	void QueuePost(string url, NetSendData sendData, System.Action<int, string> after = null, bool isFirst = false) {
		requestQueue.Add(new RequestData() { sendData = sendData, url = url, after = after, isFirst = isFirst });
	}

	public void BattleStart(Action<BattleUpdate> OnGetData) {

		QueuePost(Parametrs.Instance.apiServer + "/game/battles/battle_parser.php?xhr_ver=XMLHttpRequest&id_q=start", null, (code, resp) => {
			//Debug.Log("GOT Create Chatacter");
			try {
				var battleInfo =
					Newtonsoft.Json.JsonConvert.DeserializeObject<Network.Input.BattleUpdate>(resp);
				battleInfo.Init();
				if (OnGetData != null) OnGetData(battleInfo);
			} catch(Exception ex) {
				Debug.LogError(ex.Message);
				if (OnGetData != null) OnGetData(null);
			}
		});

	}
	
	IEnumerator queuePostWorker() {
		while (true) {
			while (requestQueue.Count > 0) {
				
				var cur = requestQueue[0];
				requestQueue.RemoveAt(0);
#if TEST_REQUEST
				PostTest(cur.url, cur.sendData, cur.after, cur.isFirst);
#else
				yield return PostWithHeaders(cur.url, cur.sendData, cur.after);
#endif
				yield return new WaitForSeconds(0.1f);
			}
			yield return new WaitForSeconds(0.2f);
		}
	}

	public void TxtRequest(string queryReauest, Action<string> OnComplited) {

		QueuePost(Parametrs.Instance.apiServer + queryReauest, null, (code, resp) => {
			if (OnComplited != null) OnComplited(resp);
		});

	}


	/// <summary>
	/// Получение информации о враге
	/// </summary>
	/// <param name="enemyId"></param>
	/// <param name="OnComplited"></param>
	public void GetEnemyInfo(int enemyId, Action<EnemyData> OnComplited) {
		
		string queryUrl = String.Format("/game/eninfo_parser.php?xhr_ver=XMLHttpRequest&id_query=Battle_refresh&enemy_id={0}&all=1", enemyId);

		QueuePost(Parametrs.Instance.apiServer + queryUrl, null, (code, resp) => {
			Debug.Log(resp);
			EnemyData enemyInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<Network.Input.EnemyData>(resp);
			if (OnComplited != null) OnComplited(enemyInfo);
		});
	}

	public void CustomRequest(string url, Action<WWW> callback) {
		StartCoroutine(CustomRequestCor(url, callback));
	}


	public IEnumerator CustomRequestCor(string url, Action<WWW> callback) {

		string queryUrl = Parametrs.Instance.apiServer + url;

		if (!String.IsNullOrEmpty(_tok))
			url += "&tok=" + _tok;
		
		WWW www =
#if UNITY_EDITOR
			new WWW(queryUrl, null, GetHeaders());
#else
			new WWW(queryUrl, null, GetHeaders());
#endif

		yield return www;

		if (callback != null) callback(www);

	}

	/// <summary>
	/// Движение игрока
	/// </summary>
	/// <param name="posit"></param>
	/// <param name="OnComplited"></param>
	public void PlayerMoveTo(Vector2 posit, ClickType typeClick, Action<Network.Input.BattleUpdate> OnComplited) {

		string queryUrl = String.Format("/game/battles/battle_parser.php?xhr_ver=XMLHttpRequest&id_q={0}&data[0]={1}&data[1]={2}", typeClick.ToString(), posit.x, posit.y);

		QueuePost(Parametrs.Instance.apiServer + queryUrl, null, (code, resp) => {
			Debug.Log(Parametrs.Instance.apiServer + queryUrl);
			Debug.Log(resp);
			var battleInfo =
				Newtonsoft.Json.JsonConvert.DeserializeObject<Network.Input.BattleUpdate>(resp);
			battleInfo.Init();
			if (OnComplited != null) OnComplited(battleInfo);
		});
	}

	public enum ClickType {
		none,
		move,
		magic_cell,
		attack_cell
	}

	public void EndRound(string queryReauest, Action<string> OnComplited) {

		string queryUrl = String.Format("/game/battles/attack_parser.php?xhr_ver=XMLHttpRequest{0}", queryReauest);

		QueuePost(Parametrs.Instance.apiServer + queryUrl, null, (code, resp) => {
			if (OnComplited != null) OnComplited(resp);
		});

	}

	public void Attack(string queryReauest, Action<string> OnComplited) {

		// /game/battles/attack_parser.php?xhr_ver=XMLHttpRequest&id_query=complete?data[type]=1&data[kicks][1][0]=10&data[kicks][2][0]=0&data[kicks][3][0]=0&data[blocks][1][0]=0&data[blocks][1][1]=0&data[blocks][2][0]=10&data[blocks][2][1]=20&data[blocks][3][0]=0&data[blocks][3][1]=0&data[blocks][3][2]=0&data[blocks][3][3]=0

		string queryUrl = String.Format("/game/battles/attack_parser.php?xhr_ver=XMLHttpRequest{0}", queryReauest);

		Debug.Log("Attack: " + (Parametrs.Instance.apiServer + queryUrl));

		QueuePost(Parametrs.Instance.apiServer + queryUrl, null, (code, resp) => {
			Debug.Log("Response: " + resp);
			if (OnComplited != null) OnComplited(resp);
		});
	}

	public void BattleUpdate(string battleId, Action<BattleUpdate> OnComplited) {
		// /game/battles/battle_parser.php?xhr_ver=XMLHttpRequest&id_q=timer&battle_id=9741080

		string queryUrl = String.Format("/game/battles/battle_parser.php?xhr_ver=XMLHttpRequest&id_q=timer&battle_id={0}", battleId);

		QueuePost(Parametrs.Instance.apiServer + queryUrl, null, (code, resp) => {

			var battleInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<Network.Input.BattleUpdate>(resp);
			//var battleInfo = (Network.Input.BattleUpdate)MiniJSON.Json.Deserialize(resp);

			battleInfo.Init();
			if (OnComplited != null) OnComplited(battleInfo);
		});

	}

	public void TechnicEndRound(Action<string> OnComplited) {
		string queryUrl = "/game/battles/battle_parser.php?xhr_ver=XMLHttpRequest&id_q=technic_action&data=complete";
		QueuePost(Parametrs.Instance.apiServer + queryUrl, null, (code, resp) => {
			if (OnComplited != null) OnComplited(resp);
		});
	}

	public void TechnicLeave(Action<string> OnComplited) {
		string queryUrl = "/game/battles/battle_parser.php?xhr_ver=XMLHttpRequest&id_q=technic_action&data=leave";
		QueuePost(Parametrs.Instance.apiServer + queryUrl, null, (code, resp) => {
			if (OnComplited != null) OnComplited(resp);
		});
	}

	[ContextMenu("Create Magic Clone")]
	public void CreateMagic() {

		Magic send = new Magic() {
			act = 15,
			behaviour = 0
		};

		string queryUrl = "/game/window/ability/magic_ability.php?xhr_ver=XMLHttpRequest";

		QueuePost(Parametrs.Instance.apiServer + queryUrl, send, (code, resp) => {
			Debug.Log(resp);
		});
	}

	[ContextMenu("Create Ballistic")]
	public void CreateBallistic() {

		Ballistic send = new Ballistic() {
			id_q = "middleTechnic",
			id = 1
		};

		string queryUrl = "/game/window/technics/technics_parser.php?xhr_ver=XMLHttpRequest&id_q=middleTechnic&data['curr_technic']=1";

		QueuePost(Parametrs.Instance.apiServer + queryUrl, null, (code, resp) => {
			Debug.Log(resp);
		});

	}

	public void GetItemImage(string classId, string modelId, string idemId, string hand, Action<Texture2D> OnComplited) {

		CustomRequest(String.Format("/tools/items/get_img.php?class_id={0}&model={1}&item_id={2}&hand={3}", classId, modelId, idemId, hand), (data) => {

			Texture2D useTexture = new Texture2D(data.texture.width, data.texture.height, TextureFormat.RGBA32, false);
			useTexture.SetPixels(data.texture.GetPixels(0));
			useTexture.Apply(false);
			if (OnComplited != null) OnComplited(useTexture);


		});
	}

	public void GetEnemyImage(string classId, string modelId, string raceId, Action<Texture2D> OnComplited) {
		CustomRequest(String.Format("/tools/items/get_img.php?class_id={0}&model={1}&race={2}&type=pl", classId, modelId, raceId), (data) => {

			Texture2D useTexture = new Texture2D(data.texture.width, data.texture.height, TextureFormat.RGBA32, false);
			useTexture.SetPixels(data.texture.GetPixels(0));
			useTexture.Apply(false);
			if (OnComplited != null) OnComplited(useTexture);

		});
	}

}

[System.Serializable]
public abstract class NetSendData {

	public string Serializable() {
		return Newtonsoft.Json.JsonConvert.SerializeObject(this);
	}

	public abstract void AddFieldsBest();

	public abstract WWWForm GetForm();

}

[System.Serializable]
public class AuthSendData : NetSendData {
	public string action;
	public string login;
	public string password;

	public override WWWForm GetForm() {
		WWWForm rtm = new WWWForm();
		rtm.AddField("action", action);
		rtm.AddField("login", login);
		rtm.AddField("password", password);
		return rtm;
	}

	public override void AddFieldsBest() {
		/*
		req.AddField("action", action);
		req.AddField("login", login);
		req.AddField("password", password);
		*/
	}

}

public class Magic : NetSendData {
	public int act;
	public int behaviour;

	public override void AddFieldsBest() {
		throw new NotImplementedException();
	}

	public override WWWForm GetForm() {
		WWWForm rtm = new WWWForm();
		rtm.AddField("act", act);
		rtm.AddField("behaviour", behaviour);
		return rtm;
	}
}

public class Ballistic : NetSendData {
	public string id_q;
	public int id;

	public override void AddFieldsBest() {
		throw new NotImplementedException();
	}

	public override WWWForm GetForm() {
		WWWForm rtm = new WWWForm();
		rtm.AddField("id_q", id_q);
		rtm.AddField("data['curr_technic']", id);
		return rtm;
	}
}

public class NetJsAnswer {
	public int id;
	public string code;
	public string text;
}