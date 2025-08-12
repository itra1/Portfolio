using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

#if PLUGIN_FACEBOOK
using Facebook.Unity;
#endif
/// <summary>
/// Друг из FB
/// </summary>
[System.Serializable]
public struct userFriendsFb {
	public string id;
	public string name;
	public string score;
}

/// <summary>
/// Facebook
/// </summary>
public class FBController : MonoBehaviour {

	// Ссылка на экземпляр
	public static FBController instance;

	// Флаг авторизации с правом публикации
	bool publicPermission = false;

	// Имя пользователя
	[HideInInspector]
	public string userName;

	// E-mail пользователя
	[HideInInspector]
	public string userEmail;

	// Аватар пользователя
	[HideInInspector]
	public string userPictureUrl;

	// Бизнес токен пользователя
	[HideInInspector]
	public string token_for_business;

	// Список друзей
	[HideInInspector]
	public List<userFriendsFb> friendList;

	public static event EventAction authorizationComplited;

	public delegate void resulatComand();
	resulatComand rerultComplitedComand;

	public delegate void resultFriends();
	resultFriends resutlFriends;

	public delegate void InviteFriendsDelegate(int count, List<ErrorInvate> friends);
	InviteFriendsDelegate resultInvite;

	/// <summary>
	/// Проверка авторизации
	/// </summary>
	public static bool CheckFbLogin {
		get {
#if PLUGIN_FACEBOOK
			return FB.IsLoggedIn;
#else
            return false;
#endif
		}
	}

	/// <summary>
	/// Текущий id facebook
	/// </summary>
	public static string GetUserId {
		get {
#if PLUGIN_FACEBOOK
			return AccessToken.CurrentAccessToken.UserId;
#else
            return "";
#endif
		}
	}

	/// <summary>
	/// Инициализация
	/// </summary>
	void Awake() {
#if PLUGIN_FACEBOOK

		DontDestroyOnLoad(this);
		instance = this;

		if (!FB.IsInitialized)
			FB.Init(OnInit);
#else
    
    Destroy(gameObject);
    return;

#endif
	}

	/// <summary>
	/// Инифиализация
	/// </summary>
	void OnInit() {

		StartCoroutine(WaitAndDownload());
	}

	IEnumerator WaitAndDownload() {
#if PLUGIN_FACEBOOK
		int ii = 0;
		while (ii < 240) {
			ii++;
			yield return new WaitForSeconds(0.5f);
			if (FB.IsInitialized && FB.IsLoggedIn && userName == "") {
				PreloadUserData();
			} else if (userName != "") {
				ii = 240;
			}
		}
#else
        yield return null;
#endif
	}

#if PLUGIN_FACEBOOK
	void fillUser(IGraphResult result) {

		if (result.Error != null && result.Error.Length > 0)
			return;

		Questions.QuestionManager.fbContact();

		Dictionary<string , object> dict = (Dictionary<string , object>)result.ResultDictionary;
		userName = dict["name"].ToString();

		try {
			userEmail = dict["email"].ToString();
		} catch {
			userEmail = AccessToken.CurrentAccessToken.UserId + "@facebook.com";
		}

		token_for_business = dict["token_for_business"].ToString();

		try {
			Dictionary<string , object> picture = (Dictionary<string , object>)dict["picture"];
			Dictionary<string , object> pictureData = (Dictionary<string , object>)picture["data"];
			userPictureUrl = pictureData["url"].ToString();
		} catch {
			Debug.Log("Нету картинки на пользователе");
		}

		try {
			Apikbs.instance.Auth(token_for_business, AccessToken.CurrentAccessToken.TokenString, userEmail);
		} catch {
			Debug.Log("Не найден класс apikbs");
		}

		if (rerultComplitedComand != null)
			rerultComplitedComand();
	}

	// Парсим список друзей
	void FillFriends(IGraphResult result) {

		if (result.Error != null) {
			Debug.Log(result.Error);
			return;
		}

		Dictionary<string , object> dict = (Dictionary<string , object>)result.ResultDictionary;
		List<object> friendData = (List<object>)dict["data"];

		friendList = new List<userFriendsFb>();

		foreach (Dictionary<string, object> friend in friendData) {
			Dictionary<string , object> friendOne = (Dictionary<string , object>)friend["user"];
			userFriendsFb frienItem = new userFriendsFb();
			frienItem.id = friendOne["id"].ToString();
			frienItem.name = friendOne["name"].ToString();
			friendList.Add(frienItem);
		}
	}

	/// <summary>
	/// Авторизация
	/// </summary>
	/// <param name="delegateFunction">Функция вызова ответной авторизации</param>
	public static void fbLogin(resulatComand delegateFunction) {
		if (instance == null)
			return;
		instance.FBlogin(delegateFunction);
	}

	/// <summary>
	/// Авторизация
	/// </summary>
	/// <param name="delegateFunction">Функция вызова ответной авторизации</param>
	public void FBlogin(resulatComand delegateFunction) {
		rerultComplitedComand = delegateFunction;
		FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, FbLoginCalbac);
	}

	/// <summary>
	/// Авторизация с правом редактирования данных
	/// </summary>
	void LoginInWritePermission() {
		FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, FbLoginWriteCalbac);
	}

	void FbLoginCalbac(ILoginResult loginresult = null) {

		if (loginresult.Error == null) {

			YAppMetrica.Instance.ReportEvent("Шаринг: вход через Facebook");

			GAnalytics.Instance.LogEvent("Шаринг", "Вход", "Вход через Facebook", 1);

		}

		int loginCount = PlayerPrefs.GetInt("loginFacebook" , 0);
		PlayerPrefs.SetInt("loginFacebook", ++loginCount);
		if (loginCount == 1) {
			InviteFriends();
		}

		PreloadUserData(loginresult);
	}

	void PreloadUserData(ILoginResult loginresult = null) {
		if (authorizationComplited != null)
			authorizationComplited();
		FBGetUserProfil(fillUser);
		GetFriends(FillFriends);
	}

	/// <summary>
	/// Получение аватарки друзей
	/// </summary>
	/// <param name="facebookID">Идентификатор друга</param>
	/// <param name="width">Ширина аватарки друга</param>
	/// <param name="height">Высота аватарки друга</param>
	/// <param name="type">Хз что это</param>
	/// <returns></returns>
	public static string GetPictureURL(string facebookID, int? width = null, int? height = null, string type = null) {
		string url = string.Format("/{0}/picture" , facebookID);
		string query = width != null ? "&width=" + width.ToString() : "";
		query += height != null ? "&height=" + height.ToString() : "";
		query += type != null ? "&type=" + type : "";
		query += "&redirect=false";
		if (query != "")
			url += ("?g" + query);
		return url;
	}

	// Получение собственного аватара
	public static void FBGetUserAvatar(FacebookDelegate<IGraphResult> delegateFunction) {
		FB.API(GetPictureURL("me", 40, 40), HttpMethod.GET, delegateFunction);
	}

	public static void FBGetFriendAvatar(string ids, FacebookDelegate<IGraphResult> delegateFunction) {

		FB.API(GetPictureURL(ids, 40, 40), HttpMethod.GET, delegateFunction);
	}

	public static string DeserializePictureURLString(string response) {
		return DeserializePictureURLObject(Json.Deserialize(response));
	}

	// Получение собственных данных
	public static void FBGetUserProfil(FacebookDelegate<IGraphResult> delegateFunction) {
		FB.API("/me?fields=name,email,token_for_business,picture", HttpMethod.GET, delegateFunction);
	}

	public static void FBGetFriendName(string id, FacebookDelegate<IGraphResult> delegateFunction) {
		FB.API("" + id, HttpMethod.GET, delegateFunction);
	}

	public static string DeserializePictureURLObject(object pictureObj) {
		var picture = (Dictionary<string , object>)( ( (Dictionary<string , object>)pictureObj )["data"] );
		object urlH = null;
		if (picture.TryGetValue("url", out urlH)) {
			return (string)urlH;
		}

		return null;
	}

	// Инвайт друзей
	#region Invite

	public void InvateFriendFb() {
		InviteFriends(ProcessInvation);
	}

	public void InviteFriends(InviteFriendsDelegate delegateFunction = null) {
		if (delegateFunction != null)
			resultInvite = delegateFunction;
		string requestMessage = "The only one game from the King Bird Games studio that encourage stealing! Invite your friends! And get to know who runs faster!";
		FB.AppRequest(requestMessage, null, null, null, null, null, null, ResultInviteCalback);
	}

	List<Dialoglist> listDialog = new List<Dialoglist>();
	void ProcessInvation(int count, List<ErrorInvate> friendsError) {
		listDialog.Clear();
		foreach (ErrorInvate friend in friendsError) {
			Dialoglist item = new Dialoglist();
			item.idFb = friend.id;
			item.name = friend.name;
			item.type = friend.error;
			listDialog.Add(item);
		}

		if (count > 0) {
			Dialoglist item1 = new Dialoglist();
			item1.idFb = "";
			item1.coinsCount = count * 100;
			item1.name = item1.coinsCount.ToString();
			item1.type = 0;
			listDialog.Add(item1);
			Questions.QuestionManager.fbInvite(count);
		}

		if (listDialog.Count > 0)
			ShowInvateDialog();

	}

	void ShowInvateDialog() {

		if (listDialog.Count <= 0) return;

		Dialoglist dial = listDialog[0];
		GameObject inst = UiController.ShowUi(UITypes.inviteDialog);
		inst.SetActive(true);
		InvateDialog invateDialog = inst.GetComponent<InvateDialog>();
		invateDialog.Show(dial);

		listDialog.RemoveAt(0);

		invateDialog.OnClose = () => {
			UserManager.coins += dial.coinsCount;
			if (listDialog.Count > 0) {
				inst.SetActive(false);
				ShowInvateDialog();
			} else {
				Destroy(inst);
			}
		};
	}


	public void InviteFriendsTest(InviteFriendsDelegate delegateFunction) {
		List<ErrorInvate> errorList = new List<ErrorInvate>();

		foreach (LeaderboardItem it in Apikbs.instance.LeaderboardFb) {
			ErrorInvate itm = new ErrorInvate();
			itm.id = it.fb;
			itm.name = it.firstName + " " + it.lastName;
			itm.error = 2;
			errorList.Add(itm);
		}

		foreach (LeaderboardItem it in Apikbs.instance.LeaderboardFb) {
			ErrorInvate itm = new ErrorInvate();
			itm.id = it.fb;
			itm.name = it.firstName + " " + it.lastName;
			itm.error = 1;
			errorList.Add(itm);
		}

		foreach (LeaderboardItem it in Apikbs.instance.LeaderboardFb) {
			ErrorInvate itm = new ErrorInvate();
			itm.id = it.fb;
			itm.name = it.firstName + " " + it.lastName;
			itm.error = 2;
			errorList.Add(itm);
		}

		delegateFunction(5, errorList);
	}

	void ResultInviteCalback(IAppRequestResult result) {

		Dictionary<string , object> dict = (Dictionary<string , object>)result.ResultDictionary;

		int totalCount = 0;

		List<string> totalRequest = Apikbs.instance.inviteFb;

		List<ErrorInvate> errorList = new List<ErrorInvate>();

		List<object> list;
		try {
			list = (List<object>)dict["to"];
		} catch {
			list = null;
		}

		if (list != null) {
			foreach (object listItem in list) {

				if (!totalRequest.Exists(x => x == listItem.ToString())) {
					if (!Apikbs.instance.LeaderboardFb.Exists(x => x.fb == listItem.ToString())) {
						totalCount++;
						totalRequest.Add(listItem.ToString());
					} else {
						ErrorInvate oneInv = new ErrorInvate();
						oneInv.id = listItem.ToString();
						oneInv.error = 2;
						errorList.Add(oneInv);
					}

				} else {
					ErrorInvate oneInv = new ErrorInvate();
					oneInv.id = listItem.ToString();
					oneInv.error = 1;
					errorList.Add(oneInv);

				}
			}
		}

		if (totalRequest.Count > 0) {

			YAppMetrica.Instance.ReportEvent("Шаринг: приглашено " + totalRequest.Count + " друзей");

			GAnalytics.Instance.LogEvent("Шаринг", "Приглашено", "Приглашено " + totalRequest.Count + " друзей", 1);

		}

		// Заливаем на сервер
		StartCoroutine(Apikbs.instance.SaveInviteFb());

		// Возвращаем число
		if (resultInvite != null)
			resultInvite(totalCount, errorList);
	}

	#endregion

	/// <summary>
	/// Поучаем список друзей из Facebook
	/// </summary>
	/// <param name="delegateFunction"></param>
	public static void GetFriends(FacebookDelegate<IGraphResult> delegateFunction) {
		FB.API("/app/scores?fields=user", HttpMethod.GET, delegateFunction);
	}
	public static void ShareLink(System.Uri urlLink, System.Uri urlPhoto, string publicTitle, string publicText) {
		FB.ShareLink(urlLink
												, publicTitle
												, publicText
												, urlPhoto
												, ShareResult);

		// ВЫполнение квеста шары, если такой имеется
		//Questions.QuestionManager.fbShare();
		Questions.QuestionManager.ConfirmQuestion(Quest.shareGame, 1);
		
		YAppMetrica.Instance.ReportEvent("Шаринг: Шаринг результатов");

		GAnalytics.Instance.LogEvent("Шаринг", "Выполнено", "Шаринг результатов", 1);
		
	}

	public static void ShareResult(IShareResult result) {
		Debug.Log(result.RawResult);
	}

	public static void SaveScore() {

		if (FB.IsLoggedIn == false)
			return;

		FBController.FBGetUserProfil(SaveScoreBest);
	}

	public delegate void WritePermissionCompl();
	public WritePermissionCompl OnWritePermissionCompl;

	/// <summary>
	/// Ответ при авторизации
	/// </summary>
	/// <param name="loginresult"></param>
	void FbLoginWriteCalbac(ILoginResult loginresult = null) {

		if (loginresult.Error != null) return;
		if (OnWritePermissionCompl != null) OnWritePermissionCompl();
	}


	public static void ShareResult() {

		if (!instance.publicPermission) {
			instance.OnWritePermissionCompl += instance.ShareResultPermission;
			instance.LoginInWritePermission();
		} else {
			instance.ShareResultPermission();
		}
		/*
        System.Uri urlLink = new System.Uri("https://itunes.apple.com/app/jack-rover/id1049865907");
        System.Uri urlPhoto = new System.Uri("http://upinapp.com/uploads/applications/30/splash_promo.jpg");
        //System.Uri urlPhoto = new System.Uri(newUrlPhoto);

        string publicTitle = "Jack Rover";
        string publicText = "Let's get to know who runs faster!";

        ShareLink(urlLink , urlPhoto , publicTitle , publicText);
        
        //FBController.instance.CreateSharePhotoAlbum();
        */
	}

	/// <summary>
	/// Подготовка на запрос к получению разрешения на редактирование
	/// </summary>
	void ShareResultPermission() {
		OnWritePermissionCompl -= ShareResultPermission;
		publicPermission = true;
		StartCoroutine(TakeScreenshot());

	}

	/// <summary>
	/// Выполняется скриншот
	/// </summary>
	/// <returns></returns>
	private IEnumerator TakeScreenshot() {
		yield return new WaitForEndOfFrame();

		var width = Screen.width;
		var height = Screen.height;
		var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();
		byte[] screenshot = tex.EncodeToPNG();

		var wwwForm = new WWWForm();
		wwwForm.AddBinaryData("image", screenshot, "InteractiveConsole.png");
		wwwForm.AddField("message", "Let's get to know who runs faster!");
		wwwForm.AddField("no_story", "true");
		FB.API("me/photos", HttpMethod.POST, ResultPublicPhotoCalback, wwwForm);

	}

	/// <summary>
	/// Шаринг результата забега
	/// </summary>
	/// <param name="newUrlPhoto"></param>
	void ShareResultConfirm(string newUrlPhoto) {

		System.Uri urlLink = new System.Uri("https://itunes.apple.com/app/jack-rover/id1049865907");
		System.Uri urlPhoto = new System.Uri(newUrlPhoto);

		string publicTitle = "Jack Rover";
		string publicText = "Let's get to know who runs faster!";

		ShareLink(urlLink, urlPhoto, publicTitle, publicText);

	}

	/// <summary>
	/// Ответ на создание фотоальбома
	/// </summary>
	/// <param name="result"></param>
	void ResultPublicPhotoCalback(IGraphResult result) {

		Dictionary<string , object> dict = (Dictionary<string , object>)result.ResultDictionary;

		if (!dict.ContainsKey("id")) return;

		GetShareLink(dict["id"].ToString());
	}

	public void GetShareLink(string photoId) {
		FB.API("/" + photoId + "?fields=link,images", HttpMethod.GET, GetShareLinkConfirm);
	}

	/// <summary>
	/// Получение ссылки на загруженное изображение
	/// </summary>
	/// <param name="result"></param>
	void GetShareLinkConfirm(IGraphResult result) {
		Dictionary<string , object> dict = (Dictionary<string , object>)result.ResultDictionary;
		List<object> imageList = (List<object>)dict["images"];
		Dictionary<string , object> oneImage = (Dictionary<string , object>)imageList[0];

		ShareResultConfirm(oneImage["source"].ToString());
	}

	/// <summary>
	/// Сохранение лучшего значения очков
	/// </summary>
	/// <param name="prof">Результат обработки</param>
	static void SaveScoreBest(IGraphResult prof) {
		Dictionary<string , object> dict = (Dictionary<string , object>)prof.ResultDictionary;

		string fbScoreStr = dict["score"].ToString();
		int fbScore = int.Parse(fbScoreStr);

		float maxDistance = UserManager.Instance.survivleMaxRunDistance;

		if (fbScore > maxDistance) {
			UserManager.Instance.survivleMaxRunDistance = fbScore;
		} else {
			var ScoreData = new Dictionary<string , string>();
			ScoreData["score"] = maxDistance.ToString();
			FB.API("/me/scores",
							HttpMethod.POST,
							delegate (IGraphResult result) {
								Debug.Log(result.RawResult);
							},
							ScoreData);
		}
	}
#else

#endif

}
