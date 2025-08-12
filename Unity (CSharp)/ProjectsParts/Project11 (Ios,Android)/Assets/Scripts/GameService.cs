using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
using UnityEngine.SocialPlatforms;
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class GameService : Singleton<GameService> {
	private bool _isAuth;
	public static event System.Action OnAuthEvent;

	public List<AchivesData> achivesList;

	private IAchievementDescription[] googleAchivmentListDescription;

	public bool isAuth {
		get {
			return _isAuth
#if UNITY_ANDROID
			       || PlayGamesPlatform.Instance.IsAuthenticated();
#elif UNITY_IOS
			|| Social.localUser.authenticated;
#else
				;
#endif
		}
		set {
			_isAuth = value;
			if (OnAuthEvent != null) OnAuthEvent();
		}
	}

	[System.Serializable]
	public struct AchivesData {
		public string key;
		public string iosKey;
		public string androidKey;

		public string GetKey() {
			return
#if UNITY_ANDROID
			androidKey;
#elif UNITY_IOS
			iosKey;
#else
				"";
#endif
		}
	}

	private void Start() {

#if UNITY_ANDROID

		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
				.RequestServerAuthCode(false)
				.RequestIdToken()
				.Build();
		
		PlayGamesPlatform.InitializeInstance(config);
		// recommended for debugging:
		PlayGamesPlatform.DebugLogEnabled = true;
		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate();

#endif

#if UNITY_IOS
		Login();
#endif

	}

	public void Save() {

		PlayerPrefs.SetString("isLogin", isAuth.ToString());

	}

	public void Load() {

		if (!PlayerPrefs.HasKey("isLogin"))
			return;

		bool auth = Boolean.Parse(PlayerPrefs.GetString("isLogin"));

		if (!auth) {
			isAuth = auth;
			return;
		}

		Login();

	}

	public void ReportAchives(string achiveKey) {
		AchivesData achive = achivesList.Find(x => x.key == achiveKey);

		try {

#if UNITY_ANDROID
			PlayGamesPlatform.Instance.ReportProgress(achive.GetKey(), 100.0, (success) => { });
#elif UNITY_IOS
			Social.ReportProgress(achive.GetKey(), 100.0f, (bool success) => { });
#endif

		} catch { }
	}

	public void Logout() {
		
#if UNITY_ANDROID
		PlayGamesPlatform.Instance.SignOut();
#endif

		isAuth = false;

		Save();

	}

	public void Login() {

		Social.localUser.Authenticate((bool success) => {
			isAuth = success;
			Save();
#if UNITY_ANDROID

			
			PlayGamesPlatform.Instance.LoadAchievementDescriptions((arr) => {
				
				if (arr == null) return;

				Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(arr));

				PlayerManager.Instance.achives.openAchives.ForEach((el) => {
					try {
						foreach (var achiv in arr) {
							try {
								if (achiv.hidden && achiv.id == achivesList.Find(x => x.key == String.Format("loc{0}", el)).GetKey())
									ReportAchives(String.Format("loc{0}", el));
							}
							catch {
							}
						}
					}
					catch(System.Exception ex) {
						Debug.Log(ex.Message);
					}

				});

			});


#elif UNITY_IOS
			Social.LoadAchievements((arr) => {});
#endif
			
		});

	}

	public void IosButtonClick() {

		if (!Social.localUser.authenticated) {
			Login();
		} else {
			Achives();
		}

	}

	public void GoogleButtonClick() {

#if UNITY_ANDROID
		if (!PlayGamesPlatform.Instance.IsAuthenticated()) {
			Login();
		} else {
			Achives();
		}
#endif

	}

	public void Achives() {
#if UNITY_ANDROID
		PlayGamesPlatform.Instance.ShowAchievementsUI();
#elif UNITY_IOS
		Social.ShowAchievementsUI();
#endif

	}


}
