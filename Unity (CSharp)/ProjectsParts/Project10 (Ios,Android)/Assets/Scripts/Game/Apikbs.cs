using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using MiniJSON;

#if PLUGIN_VOXELBUSTERS
using VoxelBusters.NativePlugins;
#endif

[System.Serializable]
public struct LeaderboardItem {
	public string id;
	public string fb;
	public string name;
	public string firstName;
	public string lastName;
	public string bestDistantion;
	public string picture;
	public string ava;
}

[System.Serializable]
public struct ErrorInvate {
	public string id;
	public string name;
	public string picture;
	public int error;
}

[System.Serializable]
public struct AdContent {
	public string title;
	public string url;
	public string dialog;
}

/// <summary>
/// Контроллер работы с API KBS
/// </summary>
public class Apikbs : Singleton<Apikbs> {

	public static event System.Action OnLogin;

	public static bool isLogin = false;

	[HideInInspector]
	public string access_token;
	
	private string api_key_ = "Gxbhuv4mhqiJ8QkFCtpWVzSNSsufKX2pzWlZucC_14HZKqkxdS6QzfBrwOA8JnbtU3OCKAMTQ0NDkxMTUzMw==";
	private string url_ = "http://api.upinapp.com";
	private string social_ = "fb";
	private string push_token;

	private int bestDistanceLiderboardId = 3;

	public List<string> inviteFb;                          // Список Приглашенных из ФБ
	public List<LeaderboardItem> LeaderboardFb;            //Лидерборд по ФБ
	public LeaderboardItem nearFriend;            //Лидерборд по ФБ
	
	public System.Action<List<LeaderboardItem>> returnLiderboard;                                  // Исполняемая функция
	public System.Action<AdContent> returnAdContents;                                  // Исполняемая функция

	public AdContent adContent;
	
	void Start() {
#if PLUGIN_VOXELBUSTERS
		VoxelBusters.NativePlugins.NotificationService.DidFinishRegisterForRemoteNotificationEvent += DidFinishRegisterForRemoteNotificationEvent;
#endif
	}

	private void Update() {
		if (!isLogin && FB.IsLoggedIn) {
			isLogin = true;
			if (OnLogin != null) OnLogin();
		}
	}



	#region Auth

	public void Auth(string fb_id, string access_token_fb, string mail) {
		StartCoroutine(Authorix(fb_id, access_token_fb, mail));
	}

	IEnumerator Authorix(string fb_id, string access_token_fb, string mail) {
		WWWForm form = new WWWForm();
		form.AddField("social", social_);
		form.AddField("user_id", fb_id);
		form.AddField("email", mail);
		form.AddField("access_token", access_token_fb);
		form.AddField("api_key", api_key_);
		WWW w = new WWW(url_ + "/v1/login", form);
		yield return w;
		ParseAuth(w.text);
	}

	void ParseAuth(string authText) {
		Debug.Log(authText);
		Dictionary<string, object> dict = Json.Deserialize(authText) as Dictionary<string, object>;

		if (dict["status"].ToString() != "200") {
			Debug.Log("Error auth birth server: " + authText);
		}

		if (!dict.ContainsKey("data"))
			return;
		
		Dictionary<string, object> data = (Dictionary<string, object>)dict["data"];
		access_token = data["access_token"].ToString();

		downloadAllInfo();
	}

	void downloadAllInfo() {
		// Подгружаем лидербоард
		StartCoroutine(GetLeaderboardFb());
		// Подгружаем список приглашенных
		StartCoroutine(GetInviteFb());
	}
	#endregion

	#region InviteFb

	IEnumerator GetInviteFb() {
		WWW w = new WWW(url_ + "/v1/storage?access_token=" + access_token);
		yield return w;
		GetInviteFbParse(w.text);
	}

	void GetInviteFbParse(string result) {

		Dictionary<string, object> dict = Json.Deserialize(result) as Dictionary<string, object>;


		if (dict["status"].ToString() != "200") {
			Debug.Log("Error: " + result);
		}

		if (!dict.ContainsKey("data"))
			return;

		Dictionary<string, object> data = (Dictionary<string, object>)dict["data"];
		Dictionary<string, object> dataStorage = (Dictionary<string, object>)data["data"];

		inviteFb = new List<string>();

		string totalInvite = dataStorage["InviteFb"].ToString();

		string[] totalInviteSplit = totalInvite.Split(new char[] { ',' });
		foreach (string totalInviteSpliteOne in totalInviteSplit) {
			inviteFb.Add(totalInviteSpliteOne);
		}

	}

	public IEnumerator SaveInviteFb() {

		string totalInvite = string.Join(",", inviteFb.ToArray());

		WWWForm form = new WWWForm();
		form.AddField("InviteFb", totalInvite);

		WWW w = new WWW(url_ + "/v1/storage?access_token=" + access_token, form);

		yield return w;
	}

	public void ResetInvates() {
		StartCoroutine(ResetInvatesCor());
	}

	public IEnumerator ResetInvatesCor() {
		int count = 500;
		int num = 0;

		while (num < count) {
			num++;
			if (access_token == null | access_token == "") {
				yield return new WaitForEndOfFrame();
			} else {
				num = count;
			}
		}

		WWWForm form = new WWWForm();
		form.AddField("InviteFb", "");

		WWW w = new WWW(url_ + "/v1/storage?access_token=" + access_token, form);

		yield return w;
		inviteFb = new List<string>();
		StartCoroutine(GetInviteFb());
	}

	#endregion

	#region LeaderBoardFb

	public void GetLeaderBoard(System.Action<List<LeaderboardItem>> func = null) {

		StartCoroutine(GetLeaderboardFb(func));
	}

	bool waitDownloadsFriend = false;

	public IEnumerator GetLeaderboardFb(System.Action<List<LeaderboardItem>> callback = null) {

		if (!waitDownloadsFriend) {
			waitDownloadsFriend = true;

			int count = 500;
			int num = 0;

			while (num < count) {
				num++;
				if (access_token == null | access_token == "") {
					yield return new WaitForEndOfFrame();
				} else {
					num = count;
				}
			}

#if UNITY_EDITOR
			WWW w = new WWW(url_ + "/v1/leaderboard?access_token=" + access_token);
#else
            WWW w = new WWW(url_ + "/v1/leaderboard?access_token=" + access_token + "&filter=friends");
#endif

			yield return w;
			waitDownloadsFriend = false;
			GetLeaderboardFbParse(w.text, callback);
		}
	}

	void GetLeaderboardFbParse(string result, System.Action<List<LeaderboardItem>> callback = null) {

		return;

		Dictionary<string, object> dict = Json.Deserialize(result) as Dictionary<string, object>;

		if (dict["status"].ToString() != "200") {
			Debug.Log("Error auth birth server: " + result);
			return;
		}
		if (!dict.ContainsKey("data"))
			return;

		int MaxPlayerDistance = PlayerPrefs.GetInt("maxDistance");

		bool isPlayer = false;

		LeaderboardFb = new List<LeaderboardItem>();
		nearFriend = new LeaderboardItem();

		List<object> dataLiderboards = (List<object>)dict["data"];

		foreach (object dataLiderboard in dataLiderboards) {

			List<object> items = (List<object>)((Dictionary<string, object>)dataLiderboard)["items"];

			foreach (Dictionary<string, object> lider in items) {
				LeaderboardItem oneItem = new LeaderboardItem();
				oneItem.id = lider["id"].ToString();
				oneItem.bestDistantion = lider["totalscore"].ToString();
				Dictionary<string, object> playerData = (Dictionary<string, object>)lider["player"];
				oneItem.name = playerData["firstname"].ToString() + " " + playerData["lastname"].ToString();
				oneItem.firstName = playerData["firstname"].ToString();
				oneItem.lastName = playerData["lastname"].ToString();
				oneItem.fb = playerData["fb"].ToString();
				Dictionary<string, object> avatarData = (Dictionary<string, object>)playerData["avatar"];
				Dictionary<string, object> squareAvatarData = (Dictionary<string, object>)avatarData["square"];
				oneItem.picture = squareAvatarData["x48"].ToString();
				LeaderboardFb.Add(oneItem);

				if (!isPlayer) {
					nearFriend.id = oneItem.id;
					nearFriend.name = oneItem.name;
					nearFriend.firstName = oneItem.firstName;
					nearFriend.lastName = oneItem.lastName;
					nearFriend.fb = oneItem.fb;
					nearFriend.bestDistantion = oneItem.bestDistantion;
					nearFriend.picture = oneItem.picture;
				}

				if (oneItem.fb == FBController.GetUserId) {
					isPlayer = true;
					if (int.Parse(oneItem.bestDistantion) > MaxPlayerDistance) {
						PlayerPrefs.SetInt("maxDistance", int.Parse(oneItem.bestDistantion));
						MaxPlayerDistance = int.Parse(oneItem.bestDistantion);

						GameManager.updateGate();
					} else {
#if UNITY_IOS || UNITY_ANDROID
						SaveLeaderboardFbValue(MaxPlayerDistance);
#endif
					}
				}

			}
		}


		if (callback != null) {
			callback(LeaderboardFb);
		}
	}


	public void SaveLeaderboardFbValue(int newValue) {
		StartCoroutine(SaveLeaderboardFbValueCor(newValue));
	}

	IEnumerator SaveLeaderboardFbValueCor(int newValue) {
		int count = 500;
		int num = 0;

		while (num < count) {
			num++;
			if (access_token == null | access_token == "") {
				yield return new WaitForEndOfFrame();
			} else {
				num = count;
			}
		}

		WWWForm form = new WWWForm();
		form.AddField("leaderboard_id", bestDistanceLiderboardId);
		form.AddField("score", newValue);
		WWW w = new WWW(url_ + "/v1/leaderboard?access_token=" + access_token, form);
		yield return w;
	}

	#endregion

	#region Push




#if PLUGIN_VOXELBUSTERS

	
	public void SubscribePush() {
		NPBinding.NotificationService.RegisterNotificationTypes(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
	}


	public static void SubscribePushIOS() {

		if (PlayerPrefs.GetInt("notificationRemoteRegister", 0) <= 0) {
			NPBinding.NotificationService.RegisterNotificationTypes(VoxelBusters.NativePlugins.NotificationType.Sound);
			NPBinding.NotificationService.RegisterForRemoteNotifications();

		}

	}

	int allSubscribe;

	public void DidFinishRegisterForRemoteNotificationEvent(string _deviceToken, string _error) {

		if (_deviceToken == "") {
			Debug.Log("Error Push Registr: " + _error);
			return;
		}

		PlayerPrefs.SetInt("notificationRemoteRegister", 1);

		push_token = _deviceToken;

		GameManager.CheckPush();

		if (allSubscribe == 0) {
			allSubscribe++;
			NPBinding.NotificationService.RegisterNotificationTypes(VoxelBusters.NativePlugins.NotificationType.Alert);
			NPBinding.NotificationService.RegisterForRemoteNotifications();
		} else if (allSubscribe == 1) {
			allSubscribe++;
			NPBinding.NotificationService.RegisterNotificationTypes(VoxelBusters.NativePlugins.NotificationType.Badge);
			NPBinding.NotificationService.RegisterForRemoteNotifications();
			DailyBonus.PushConfirm();
		}

		StartCoroutine(RegistrPushIOS());

	}

	IEnumerator RegistrPushIOS() {
		WWWForm form = new WWWForm();
		form.AddField("token", push_token);
		form.AddField("platform", "ios");
		form.AddField("api_key", api_key_);
		WWW w = new WWW(url_ + "/v1/push?action=subscribe", form);
		yield return w;
	}

	// Запись в рассписание пользовательского пуша
	public static string CreateLocalNotification(long deferredSeconds, eNotificationRepeatInterval repeatInterval, string title, string text) {

		CrossPlatformNotification _notification = new CrossPlatformNotification();
		_notification.AlertBody = text;
		_notification.FireDate = System.DateTime.Now.AddSeconds(deferredSeconds);
		_notification.SoundName = "jack_message_01.mp3"; //Держать в папке Assets/PluginResources/Android, iOS или Common.
		_notification.RepeatInterval = repeatInterval;

		IDictionary _userInfo = new Dictionary<string, string>();
		_userInfo["data"] = "custom data";

		_notification.UserInfo = _userInfo;

		CrossPlatformNotification.iOSSpecificProperties _iosProperties = new CrossPlatformNotification.iOSSpecificProperties();
		_iosProperties.HasAction = true;
		_iosProperties.AlertAction = "Ho-ho";
		_iosProperties.LaunchImage = "Icon-60.png";

		CrossPlatformNotification.AndroidSpecificProperties _androidProperties = new CrossPlatformNotification.AndroidSpecificProperties();
		_androidProperties.ContentTitle = title;
		_androidProperties.TickerText = text;
		_androidProperties.LargeIcon = "Icon-60.png"; //Держать в папке Assets/PluginResources/Android, iOS или Common.

		_notification.iOSProperties = _iosProperties;
		_notification.AndroidProperties = _androidProperties;

		string notificationID = NPBinding.NotificationService.ScheduleLocalNotification(_notification);

		return notificationID;
	}

	public static void ClearAllLocalNotification() {
		NPBinding.NotificationService.CancelAllLocalNotification();
	}
	public static void ClearLocalNotification(string notification) {
		NPBinding.NotificationService.CancelLocalNotification(notification);
	}
#endif
	#endregion

	#region AdContent
	public void GetAd(System.Action<AdContent> returnAdContents) {
		return;

		StartCoroutine(GetAdContent(returnAdContents));
	}

	IEnumerator GetAdContent(System.Action<AdContent> returnAdContents) {
		WWW w = new WWW(url_ + "/v1/banner?api_key=" + api_key_ + "&platform=ios");
		yield return w;
		GetAdContentParse(w.text, returnAdContents);
	}

	public void GetAdContentParse(string result, System.Action<AdContent> returnAdContents) {

		Dictionary<string, object> dict = Json.Deserialize(result) as Dictionary<string, object>;

		if (dict["status"].ToString() != "200") {
			Debug.Log("Error: " + result);
		}
		if (!dict.ContainsKey("data"))
			return;

		adContent = new AdContent();

		Dictionary<string, object> data = (Dictionary<string, object>)dict["data"];

		if (data != null) {
			adContent.title = data["title"].ToString();
			Dictionary<string, object> dataStorage = (Dictionary<string, object>)data["data"];
			adContent.url = dataStorage["url"].ToString();
		} else {
			Debug.Log("No ad content");
		}
		returnAdContents(adContent);
	}

	#endregion

}
