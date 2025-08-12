using ExEvent;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NetManager: Singleton<NetManager> {

  //http://words.dev.kingbird.ru/locations?shortName=en

  public string host;
  public bool ssl;

  private bool _internetAwalable;

  public bool internetAwalable {
    get { return _internetAwalable; }
    set {
      _internetAwalable = value;
      GameEvents.NetworkChange.Call(_internetAwalable);
    }
  }

  public string server {
    get { return (ssl ? "https://" : "http://") + host; }
  }

  IEnumerator RequestWithHeaders(string url, WWWForm form, System.Action<int, Response> after = null) {
    Dictionary<string, string> customHeaders = new Dictionary<string, string>();

    customHeaders.Add("Content-Type", "application/json");
    customHeaders.Add("AppVersion", Application.version);

    WWW www = new WWW(url, form != null ? form.data : null, customHeaders);
    while (!www.isDone)
      yield return null;

#if UNITY_EDITOR
    Debug.Log("Request: " + url);
    Debug.Log("Response: " + www.text);
#endif
    Response res = JsonConvert.DeserializeObject<Response>(www.text);

    //#if UNITY_EDITOR
    //		if (after != null) after(404, null);
    //		yield break;
    //#endif

    int code = 200;

    if (String.IsNullOrEmpty(www.text))
      code = 661;
    else if (!string.IsNullOrEmpty(www.error))
      code = int.Parse(www.error.Substring(0, 3));


    if (!string.IsNullOrEmpty(www.error)) {
      if (after != null) after(code, res);
      yield break;
    }

    if (after != null) after(code, res);
  }

  private void Start() {
    InitTest();
  }

  private void Request(string url, WWWForm form, System.Action<int, Response> after = null) {
    StartCoroutine(RequestWithHeaders(server + url, form, after));
  }

  /// <summary>
  /// Получение локализованной компании
  /// </summary>
  public void GetCompanies(System.Action<List<GameCompany.Company>> OnCallback) {

    Request("/localizations", null, (code, resp) => {
      if (code == 200) {
        try {
          List<GameCompany.Company> data = JsonConvert.DeserializeObject<List<GameCompany.Company>>(resp.response.ToString());
          OnCallback(data);
        } catch {
          OnCallback(null);
        }
      } else {
        OnCallback(null);
      }
    });
  }

  /// <summary>
  /// Получение локации
  /// </summary>
  /// <param name="langKey"></param>
  /// <param name="OnCallback"></param>
  public void GetLocations(string langKey, System.Action<List<GameCompany.Location>> OnCallback) {

    Request("/locations?shortName=" + langKey, null, (code, resp) => {
      if (code == 200) {
        try {
          List<GameCompany.Location> data = JsonConvert.DeserializeObject<List<GameCompany.Location>>(resp.response.ToString());
          data.ForEach(x => x.ParseLevelType());
          OnCallback(data);
        } catch {
          OnCallback(null);
        }
      } else {
        OnCallback(null);
      }
    });
  }

  /// <summary>
  /// Получение уровней
  /// </summary>
  /// <param name="locationKey"></param>
  /// <param name="OnCallback"></param>
  public void GetLevels(int locationId, string byeToken, System.Action<List<GameCompany.Level>> OnCallback) {

    string url = "/levels?locationId=" + locationId;

#if UNITY_ANDROID && !UNITY_EDITOR
		url += "&paymentPlatform=playMarket";
		
		if (!String.IsNullOrEmpty(byeToken))
			url += "&purchaseToken=" + byeToken;

#elif UNITY_IOS || UNITY_EDITOR
    url += "&paymentPlatform=appStore";

    byte[] toBytes = Encoding.UTF8.GetBytes("94P4ru4rFFLUxxYC66JJ");
    url += "&purchaseToken=" + System.Convert.ToBase64String(toBytes);
#endif

    Request(url, null, (code, resp) => {
      if (code == 200) {
        try {
          List<GameCompany.Level> data = JsonConvert.DeserializeObject<List<GameCompany.Level>>(resp.response.ToString());
          OnCallback(data);
        } catch {
          OnCallback(null);
        }
      } else {
        OnCallback(null);
      }
    });
  }

  public void GetOneLevel(int locationId, int levelNum, string accessToken, Action<GameCompany.Level> OnCallback) {
    levelNum++;

    string url = string.Format("/level?locationId={0}&levelNum={1}&accessToken={2}", locationId, levelNum, accessToken);

    Request(url, null, (code, resp) => {
      if (code == 200) {
        try {
          GameCompany.Level data = JsonConvert.DeserializeObject<GameCompany.Level>(resp.response.ToString());
          OnCallback(data);
        } catch {
          OnCallback(null);
        }
      } else {
        OnCallback(null);
      }
    });

  }

  public void GetTime(Action<string> OnCallback, Action OnFailed) {
    Request("/time", null, (code, resp) => {
      if (code == 200) {
        try {
          DateTimeServer data = JsonConvert.DeserializeObject<DateTimeServer>(resp.response.ToString());
          OnCallback(data.server_time);
        } catch {
          OnFailed();
        }
      } else {
        OnFailed();
      }
    });
  }

  public void CheckPayment(string receipt, System.Action<bool> OnCallback, Action OnFailed) {
    
    string url = "/payment/status";

#if UNITY_ANDROID && !UNITY_EDITOR
		url += "?paymentPlatform=playMarket";
		byte[] toBytes = Encoding.UTF8.GetBytes(BillingManager.Instance.ParceReceipt(receipt));
		url += "&purchaseToken=" + System.Convert.ToBase64String(toBytes);
#elif UNITY_IOS || UNITY_EDITOR
    url += "?paymentPlatform=appStore";

    byte[] toBytes = Encoding.UTF8.GetBytes("94P4ru4rFFLUxxYC66JJ");
    url += "&purchaseToken=" + System.Convert.ToBase64String(toBytes);
#endif

    Request(url, null, (code, resp) => {
      if (code == 200) {
        try {
          CheckPayment data = JsonConvert.DeserializeObject<CheckPayment>(resp.response.ToString());
          OnCallback(data.valid);
        } catch {
          OnFailed();
        }
      } else {
        OnFailed();
      }
    });
  }

  private const bool allowCarrierDataNetwork = true;
  private const string pingAddress = "8.8.8.8"; // Google Public DNS server
  private const float waitingTime = 1.0f;

  private Ping ping;
  private float pingStartTime;

  private void Update() {
    UpdateCheck();
  }

  private void OnApplicationPause(bool pause) {
    if (pause == false) {
      InitTest();
    }
  }

  private void InitTest() {
    bool internetPossiblyAvailable;
    switch (Application.internetReachability) {
      case NetworkReachability.ReachableViaLocalAreaNetwork:
        internetPossiblyAvailable = true;
        break;
      case NetworkReachability.ReachableViaCarrierDataNetwork:
        internetPossiblyAvailable = allowCarrierDataNetwork;
        break;
      default:
        internetPossiblyAvailable = false;
        break;
    }
    if (!internetPossiblyAvailable) {
      InternetIsNotAvailable();

      if (IsInvoking("StartNextPing")) return;
      Invoke("StartNextPing", 10);

      return;
    }
    ping = new Ping(pingAddress);
    pingStartTime = Time.time;
  }

  private void UpdateCheck() {
    if (ping != null) {
      bool stopCheck = true;
      if (ping.isDone)
        InternetAvailable();
      else if (Time.time - pingStartTime < waitingTime)
        stopCheck = false;
      else
        InternetIsNotAvailable();
      if (stopCheck) {
        ping = null;
        if (IsInvoking("StartNextPing")) return;
        Invoke("StartNextPing", 10);
      }
    }
  }

  private void StartNextPing() {
    InitTest();
  }

  private void InternetIsNotAvailable() {
    internetAwalable = false;
  }

  private void InternetAvailable() {
    internetAwalable = true;
  }

}

[System.Serializable]
public class Response {
  public string status;
  public object response;
}

[System.Serializable]
public class DateTimeServer {
  public string server_time;
}

[System.Serializable]
public class CheckPayment {
  public bool valid;
}
