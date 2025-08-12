using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SocialNetwork {
  EDITOR,
  FB,
  VK,
  GP,
  IOS
}

namespace Generals.Network.Http {
	/// <summary>
	/// Обработка работы сети
	/// </summary>
	public class HttpApi : MonoBehaviour {

    public static HttpApi instance;

		[HideInInspector]
    public string identity_path = "http://generals.dev.kingbird.ru/identity/user/1.0/";                 // Сервер авторизации
		[HideInInspector]
    public string generals_api_path = "http://generals.dev.kingbird.ru/generals/user/1.0/";             // REST-API сервер
		[HideInInspector]
    public string application_name = "Generals";                    // Название приложения
    [HideInInspector]
    public string token;                                                   // Токен
    string accountId;                                               // Идентификатор аккаунта
    public string AccountId { get { return accountId; } }
    public static event System.Action OnLoginSucceeded;

    public class RequestData {
      public string url;
      public WWWForm form;
      public System.Action<int, string> after;
    }

    List<RequestData> requestQueue = new List<RequestData>();

    void Awake() {
      instance = this;
    }

    private void Start() { }

    void FillCommonFields(WWWForm form) {
      form.AddField("client_version", "1.0");
      form.AddField("device_type", SystemInfo.deviceType.ToString());
      form.AddField("display", Screen.height + "x" + Screen.width);
      form.AddField("os", Application.platform.ToString());
      form.AddField("osVersion", SystemInfo.operatingSystem);
    }

    IEnumerator PostWithHeaders(string url, WWWForm form, System.Action<int, string> after = null) {
      Debug.Log("Post: " + url);
      Dictionary<string, string> customHeaders = new Dictionary<string, string>();
      if(token != null && token != "")
        customHeaders.Add("X-Access-Token", token);
      customHeaders.Add("X-Application-Name", application_name);
      customHeaders.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
      
      WWW www = new WWW(url, form != null ? form.data : null, customHeaders);
      yield return www;
      string resp = null;
      if(!string.IsNullOrEmpty(www.error)) {
        if(www.error.StartsWith("401")) {
          requestQueue.Insert(0, new RequestData() { url = url, form = form, after = after });
          requestQueue.Insert(0, CreateRefreshRequestData());
          Debug.Log("Add = " + requestQueue.Count);
        } else Debug.LogError("url:" + url + " error:" + www.error);
      } else {
        Debug.Log("Server response: " + www.text);
        resp = www.text;

        int code = string.IsNullOrEmpty(www.error)?200: int.Parse(www.error.Substring(0, 3));
        if(after != null) after(code, resp);
      }
    }

    /// <summary>
    /// Создание персонажа
    /// </summary>
    /// <param name="form"></param>
    /// <param name="answer"></param>
    public void LastToken(Actione OnComplited = null) {
      //Debug.Log("Last Token");
      QueuePost(generals_api_path + "token/last", null, (code, resp) => {
        //Debug.Log("GOT Last Token");
        if(OnComplited != null) OnComplited();
      });
    }

    public void RefreshToken(Actione OnComplited = null) {
      //Debug.Log("Refresh Token");

      var form = NetworkManager.CreateForm();
      form.AddField("token", token);

      QueuePost(identity_path + "token/refresh", form, (code, resp) => {
        //Debug.Log("GOT Refresh Token");
        var lr = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthMessage>(resp);
        token = lr.token;
        if(OnComplited != null) OnComplited();
      });
    }
    
    RequestData CreateRefreshRequestData() {
      RequestData req = new RequestData();
      req.url = identity_path + "token/refresh";
      var form = new WWWForm();
      form.headers.Add("X-Application-Name", application_name);
      form.AddField("token", token);
      req.form = form;
      req.after = (code, resp) => {
        var lr = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthMessage>(resp);
        token = lr.token;
        //Debug.Log("Got new token: " + token);
      };

			return req;
    }

    void QueuePost(string url, WWWForm form, System.Action<int, string> after = null) {
      requestQueue.Add(new RequestData() { form = form, url = url, after = after });
    }
    
    #region Login

    public void LoginSocial(string socialToken, SocialNetwork socialNetwork, System.Action<AuthMessage> after) {
      WWWForm form = new WWWForm();
      form.AddField("unique_id", SystemInfo.deviceUniqueIdentifier);
      form.AddField("social_id", socialNetwork.ToString());
      form.AddField("social_token", socialToken);
      FillCommonFields(form);
      StartCoroutine(PostWithHeaders(identity_path + "auth/social", form, (code, resp) => {
        AuthMessage lr = null;
        if(resp != null) {
          lr = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthMessage>(resp);
          token = lr.token;
          Debug.Log("Got token: " + token);

          if(code == 200) {
            if(OnLoginSucceeded != null) OnLoginSucceeded();
          }
          StartCoroutine(queuePostWorker());
        }
        if(after != null) after(lr);
      }));
    }

    #endregion

    #region Character
    /// <summary>
    /// Создание персонажа
    /// </summary>
    /// <param name="form"></param>
    /// <param name="answer"></param>
    public void CreateChatacter(WWWForm form, System.Action<Character> answer = null) {
      //Debug.Log("Create Chatacter");
      QueuePost(generals_api_path + "character/create", form, (code, resp) => {
        //Debug.Log("GOT Create Chatacter");
        var character = Newtonsoft.Json.JsonConvert.DeserializeObject<Character>(resp);
        answer(character);
      });
    }

    /// <summary>
    /// Получаем данные о персонаже
    /// </summary>
    /// <param name="answer"></param>
    public void GetChatacter(System.Action<List<Character>> answer = null) {
      //Debug.Log("Get Chatacter");
      QueuePost(generals_api_path + "characters", null, (code, resp) => {
        //Debug.Log("GOT Create Chatacter");
        var character = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Character>>(resp);
        answer(character);
      });
    }

    #endregion

    #region Профиль

    /// <summary>
    /// Получение профиля
    /// </summary>
    /// <param name="after"></param>
    public void GetProfile(System.Action<GameProfile> after) {
      //Debug.Log("Get profile");
      QueuePost(generals_api_path + "profile", null, (code, resp) => {
        //Debug.Log("GOT profile");
        var profile = Newtonsoft.Json.JsonConvert.DeserializeObject<GameProfile>(resp);
        after(profile);
      });
    }

    #endregion

    #region Карты

    public void GetCardsCatalog(System.Action<List<CardLibrary>> after) {
      //Debug.Log("Get cards catalog");
      QueuePost(generals_api_path + "cards/catalog", null, (code, resp) => {
        //Debug.Log("GOT cards catalog");
        Helpers.Invoke(this, TestFunc, 190);
        var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CardLibrary>>(resp);
        after(list);
      });
    }

    void TestFunc() {
      NetworkManager.instance.apiService.GetCardsCatalog(GameDesign.instance.CardParsing);
    }

    #endregion
    
    IEnumerator queuePostWorker() {
      while(true) {
        while(requestQueue.Count > 0) {
          var cur = requestQueue[0];
          requestQueue.RemoveAt(0);
          yield return PostWithHeaders(cur.url, cur.form, cur.after);
        }
        yield return new WaitForSeconds(0.2f);
      }
    }

    public class AuthMessage : BaseResponse {
      public string token;
      public string accountId;
      public string profileId;
    }
  }

  public class BaseResponse {
    public int statusCode;
    public string errorMessage;
  }
  
}