using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif
using UnityEngine.SceneManagement;
using VoxelBusters.NativePlugins;
#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Удалить соxранения"))
			PlayerPrefs.DeleteAll();

	}
}

#endif

public class GameManager : Singleton<GameManager> {

	[HideInInspector] public GamePhase _gamePhase = GamePhase.menu;

	public static GamePhase gamePhase {
		get { return Instance._gamePhase; }
		set {
			var old = Instance._gamePhase;
			Instance._gamePhase = value;
			if (old != GamePhase.game && Instance._gamePhase == GamePhase.game) {
				AudioManager.Instance.PlayBackMusic(1);
			} else if (old == GamePhase.game && Instance._gamePhase != GamePhase.game) {
				AudioManager.Instance.PlayBackMusic(0);
			}
			ExEvent.GameEvents.OnChangeGamePhase.Call(old, value);
		}
	}

	protected override void Awake() {
		base.Awake();
		DontDestroyOnLoad(gameObject);
		
	}

	private void Start() {

		FirebaseManager.Instance.LogEvent("start");
		Application.targetFrameRate = 60;

		if (SceneManager.GetActiveScene().name == "Game") {
			UIManager.Instance.GetPanel(UiType.splash).gameObject.SetActive(true);
			GetComponent<FishManager>().Init();
		} else {
			SceneManager.sceneLoaded += (scene, mode) => {
				UIManager.Instance.GetPanel(UiType.splash).gameObject.SetActive(true);
				GetComponent<FishManager>().Init();

				StartCoroutine(RateCor());
				StartCoroutine(ShowFullScreen());

			};
		}
		
	}

	private IEnumerator ShowFullScreen() {
		yield return new WaitForSeconds(4);
		if (PlayerManager.Instance.playCount != 0 && PlayerManager.Instance.playCount != 1 && PlayerManager.Instance.playCount % 3 == 0)
			KingBird.Ads.AdsKingBird.Instance.ShowFullBanner();
	}

	private IEnumerator RateCor() {
		yield return new WaitForSeconds(4);
		if (PlayerManager.Instance.playCount == 3)
			RateUsButton();
	}
	
	private void Update() {

		if (Input.anyKeyDown && Input.GetKeyDown(KeyCode.Escape)) {
			UIManager._instance.Escape();
		}

	}

	public void MoreGames() {

#if UNITY_IOS
		storeManager.showStoreWithProduct(Properties.Instance.moreGames);
#elif UNITY_ANDROID
		Application.OpenURL("https://play.google.com/store/apps/dev?id=5415301411857753051");
		//WebView webView = new WebView();
		//webView.AutoShowOnLoadFinish = true;
		////webView.Show();
		//webView.ControlType = eWebviewControlType.CLOSE_BUTTON;
		//webView.SetFullScreenFrame();
		//webView.LoadRequest("https://play.google.com/store/apps/dev?id=5415301411857753051");
#endif
		//storeManager.showStoreWithProduct(Properties.Instance.myProduct);
		//NPBinding.Utility.OpenStoreLink(Properties.Instance.moreGames);
	}

	public void Setting() {

		if (UIManager._instance.GetPanel(UiType.shop).isActiveAndEnabled) return;

		SettingUi set = UIManager._instance.GetPanel(UiType.setting) as SettingUi;
		set.gameObject.SetActive(true);
		set.Show();
	}

	public void Shop() {

		if (UIManager._instance.GetPanel(UiType.setting).isActiveAndEnabled) return;

		ShopUi shop = UIManager._instance.GetPanel(UiType.shop) as ShopUi;
		shop.gameObject.SetActive(true);
		shop.Show();
	}

	public void GetBonus() {
		Debug.Log("GetBonus");
	}

	public void AfterSplash() {
		if (Tutorial.Instance.isTutorial) {
			//AudioManager.Instance.PlayBackMusic(1);
			Tutorial.Instance.ToGame();
		} else {
			AudioManager.Instance.PlayBackMusic(0);
			ToGame();
		}
	}

	public void ToGame() {
		BlackRound br = (BlackRound)UIManager.Instance.GetPanel(UiType.blackRound);
		br.Play(() => {
			UIManager.Instance.GetPanel(UiType.splash).gameObject.SetActive(false);
			gamePhase = GamePhase.menu;
			UIManager.Instance.Show(UiType.menu);
		});
	}

	/// <summary>
	/// Плей
	/// </summary>
	public void MainPlay() {

		if (PlayerManager.Instance.company.companies.Count <= 0 || PlayerManager.Instance.company.companies[0].locations.Count <= 0)
			return;

		BlackRound br = (BlackRound)UIManager.Instance.GetPanel(UiType.blackRound);
		br.Play(() => {
			gamePhase = GamePhase.locations;
			UIManager.Instance.GetPanel(UiType.menu).gameObject.SetActive(false);
			CameraController.Instance.transform.position = new Vector3(0, 50, -10);
			UIManager.Instance.GetPanel(UiType.locations).gameObject.SetActive(true);
		});
	}

	/// <summary>
	/// Переход на уровни
	/// </summary>
	/// <param name="OnChange"></param>
	public void ToLevels(Action OnChange) {
		BlackRound br = (BlackRound)UIManager.Instance.GetPanel(UiType.blackRound);
		br.Play(() => {
			OnChange();
			gamePhase = GamePhase.levels;
			CameraController.Instance.transform.position = new Vector3(0, 75, -10);
			UIManager.Instance.GetPanel(UiType.locations).gameObject.SetActive(false);
			UIManager.Instance.GetPanel(UiType.levels).gameObject.SetActive(true);
			LevelsWorld lw = WorldManager.Instance.GetWorld(WorldType.levels) as LevelsWorld;
			lw.SetLevels();
		});
	}

	public void ToBack() {

		BlackRound br = (BlackRound)UIManager.Instance.GetPanel(UiType.blackRound);

		if (gamePhase == GamePhase.levels) {

			br.Play(() => {

				UIManager.Instance.GetPanel(UiType.locations).gameObject.SetActive(true);
				UIManager.Instance.GetPanel(UiType.levels).gameObject.SetActive(false);
				gamePhase = GamePhase.locations;
				CameraController.Instance.transform.position = new Vector3(0, 50, -10);
				LocationsWorld lw = WorldManager.Instance.GetWorld(WorldType.locations) as LocationsWorld;
				lw.BackLocations();
				PlayerManager.Instance.Save();
			});
			return;
		}

		br.Play(() => {
			gamePhase = GamePhase.menu;
			UIManager.Instance.GetPanel(UiType.locations).gameObject.SetActive(false);
			CameraController.Instance.transform.position = new Vector3(0, 0, -10);
			UIManager.Instance.GetPanel(UiType.menu).gameObject.SetActive(true);
		});
	}

	public void PlayGame(Action OnFillBlack = null) {
		BlackRound br = (BlackRound)UIManager.Instance.GetPanel(UiType.blackRound);

		br.Play(() => {
			if (OnFillBlack != null) OnFillBlack();
			gamePhase = GamePhase.game;
			UIManager.Instance.GetPanel(UiType.levels).gameObject.SetActive(false);
			CameraController.Instance.transform.position = new Vector3(0, 100, -10);
			UIManager.Instance.GetPanel(UiType.game).gameObject.SetActive(true);
			GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
			word.Init();
		}, () => {
			GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
			//word.OnHide();
		}, () => {
			GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
			word.OnShow();
		});
	}
	// 42A6DCFF  setting.mainDescription  ОСНОВНОЙ ЯЗЫК ВСЕЙ ИГРЫ ОСНОВНОЙ ЯЗЫК ВСЕЙ ИГРЫОСНОВНОЙ ЯЗЫК ВСЕЙ ИГРЫОСНОВНОЙ ЯЗЫК ВСЕЙ ИГРЫ setting.secondDescription
	public void BackToLevels(Action OnFillBlack = null) {

		if (PlayerManager.Instance.company.isBonusLevel) {
			BackFromBonusLevel(OnFillBlack);
			return;
		}

		PlayerManager.Instance.company.CreateNoCompleteLevelPush();

		BlackRound br = (BlackRound)UIManager.Instance.GetPanel(UiType.blackRound);

		br.Play(() => {
			if (OnFillBlack != null) OnFillBlack();
			gamePhase = GamePhase.levels;
			UIManager.Instance.GetPanel(UiType.game).gameObject.SetActive(false);
			CameraController.Instance.transform.position = new Vector3(0, 75, -10);
			UIManager.Instance.GetPanel(UiType.levels).gameObject.SetActive(true);
			//LocationsWorld word = (LocationsWorld)WorldManager.Instance.GetWorld(WorldType.locations);
			LevelsWorld lw = WorldManager.Instance.GetWorld(WorldType.levels) as LevelsWorld;
			lw.SetLevels();
		});
	}

	public void EndGame(BattlePhase phase, GameCompany.Level alvl, GameCompany.Save.Level lvl) {

		if (phase == BattlePhase.win) {
			if (Tutorial.Instance.isTutorial) {
				if (Tutorial.Instance.tutorialLevel == 0) {
					Tutorial.Instance.FirstComplete();
				} else
					Tutorial.Instance.SecondComplete();

			} else if (PlayerManager.Instance.company.isBonusLevel) {
				EndBonusGame();
			} else {
				EndDefaultGame(alvl, lvl);
			}
			return;
		}

		if (phase == BattlePhase.full) {
			if (PlayerManager.Instance.company.isBonusLevel) {
				EndBonusGameFailed();
			}
			return;
		}
	}

	private bool _loadDeady = false;

	/// <summary>
	/// Конец обычной игры
	/// </summary>
	private void EndDefaultGame(GameCompany.Level alvl, GameCompany.Save.Level lvl) {

		if (_loadDeady) return;

		GameResultUi gr = (GameResultUi)UIManager.Instance.GetPanel(UiType.gameResult);
		gr.gameObject.SetActive(true);
		gr.SetData(alvl, lvl);

		gr.OnBack = () => {
			BackToLevels(() => {
				gr.gameObject.SetActive(false);
			});
		};

		gr.OnNext = () => {

			if (PlayerManager.Instance.company.CheckExistNextLevel()) {
				PlayerManager.Instance.company.actualLevelNum++;

        if (!PlayerManager.Instance.noAds
        && PlayerManager.Instance.company.actualLocationNum > 0
        && GoogleAdsMobile.Instance.NewLevelInterestionReady()) {

					GoogleAdsMobile.Instance.ShowInterstitialVideo(
            complete: (result) => {
						PlayGame(() => {
							gr.gameObject.SetActive(false);
						});
					});

				} else {
					PlayGame(() => {
						gr.gameObject.SetActive(false);
					});
				}


				

				

//#if UNITY_ANDROID
//				PlayGame(() => {
//					gr.gameObject.SetActive(false);
//				});


//				//_loadDeady = true;
//				//gr.locker.SetActive(true);
//				//CheckAndPlayLevel(PlayerManager.Instance.company.actualLevelNum, (res) => {
//				//	_loadDeady = false;
//				//	gr.locker.SetActive(false);
//				//	if (res) {
//				//		PlayGame(() => {
//				//			gr.gameObject.SetActive(false);
//				//		});
//				//	}

//				//});


//#else
//		PlayGame(() => {
//					gr.gameObject.SetActive(false);
//				});
//#endif

				return;
			}

			BackToLevels(() => {
				gr.gameObject.SetActive(false);
			});

		};
	}

	private string acceptVideoToken = "";

	public void CheckAndPlayLevel(int levelNum, Action<bool> OnComplete) {

		LocationsWorld lw = (LocationsWorld)WorldManager.Instance.GetWorld(WorldType.locations);

		// Если локация куплена - все гуд
		if (lw.islandList[PlayerManager.Instance.company.actualLocationNum].isBye) {
			OnComplete(true);
			return;
		}

		UnityAdsVideo.UnityAdsVideoData videoData = new UnityAdsVideo.UnityAdsVideoData();
		videoData.levelNum = levelNum + 1;
		videoData.locationId = lw.islandList[PlayerManager.Instance.company.actualLocationNum].location.id;

		//UnityAdsVideo.Instance.PlayVideo(videoData, (status, token) => {
		//	acceptVideoToken = token;
		//	// Если реклама пропущена, выполняем отмену
		//	if (!status) {
		//		OnComplete(false);
		//		return;
		//	}

		//	// Если реклама просмотрена полностью, снова проверяем токен
		//	NetManager._instance.GetOneLevel(lw.islandList[PlayerManager.Instance.company.actualLocationNum].location.id, levelNum, acceptVideoToken,
		//		(lv) => {

		//			// Если получили уровень, все хорошо
		//			if (lv != null) {
		//				PlayerManager._instance.company.AddOneLevelIfNeed(lw.islandList[PlayerManager.Instance.company.actualLocationNum].location.id, levelNum, lv);
		//				OnComplete(true);
		//				return;
		//			}

		//			// Иначе ошибка
		//			Debug.Log("Video token error");
		//			OnComplete(false);

		//		});

		//});



	}

	private void DailyBonusResyme() {
		PlayerManager.Instance.company.BonusResume();
	}

	private void EndBonusGame() {
		DailyBonusUi gr = (DailyBonusUi)UIManager.Instance.GetPanel(UiType.dailyBonusConfirm);
		gr.gameObject.SetActive(true);

		gr.OnBack = () => {
			//gr.gameObject.SetActive(false);
			BackFromBonusLevel();
			return;
		};

	}
	private void EndBonusGameFailed() {
		DailyBonusResultFailed gr = (DailyBonusResultFailed)UIManager.Instance.GetPanel(UiType.dailyBonusFailed);
		gr.gameObject.SetActive(true);

		gr.OnBack = () => {
			//gr.gameObject.SetActive(false);
			BackFromBonusLevel();
			return;
		};

		gr.OnOk = () => {
			DailyBonusResyme();
		};

	}

	public void EndTutorialGame() {
		TutorConfirmUi tutor = (TutorConfirmUi)UIManager.Instance.GetPanel(UiType.tutorConfirm);
		tutor.gameObject.SetActive(true);

		tutor.OnComplete = () => {

			BlackRound br = (BlackRound)UIManager.Instance.GetPanel(UiType.blackRound);

			br.Play(() => {
				Tutorial.Instance.EndTutorial();
				UIManager.Instance.GetPanel(UiType.tutorConfirm).gameObject.SetActive(false);
				UIManager.Instance.GetPanel(UiType.game).gameObject.SetActive(false);
				UIManager.Instance.GetPanel(UiType.menu).gameObject.SetActive(true);
				gamePhase = GamePhase.menu;
				CameraController.Instance.transform.position = new Vector3(0, 0, -10);
			}, null, () => {

				PlayerManager.Instance.company.CheckDownloadIfNeed();
				OctopusMain.Instance.PlayWondering();
			});
		};
	}

	public void PlayBonusLevel() {

		BlackRound br = (BlackRound)UIManager.Instance.GetPanel(UiType.blackRound);

		br.Play(() => {
			PlayerManager.Instance.company.isBonusLevel = true;
			PlayerManager.Instance.company.lastPhase = gamePhase;
			PlayerManager.Instance.company.lastBonus++;

			UIManager.Instance.GetPanel(UiType.bonusLevelStart).gameObject.SetActive(false);

			switch (gamePhase) {
				case GamePhase.menu:
					UIManager.Instance.GetPanel(UiType.menu).gameObject.SetActive(false);
					break;
				case GamePhase.locations:
					UIManager.Instance.GetPanel(UiType.locations).gameObject.SetActive(false);
					break;
				case GamePhase.levels:
					UIManager.Instance.GetPanel(UiType.levels).gameObject.SetActive(false);
					break;
			}

			CameraController.Instance.transform.position = new Vector3(0, 100, -10);
			UIManager.Instance.GetPanel(UiType.game).gameObject.SetActive(true);

			gamePhase = GamePhase.game;

			GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
			word.Init();

		}, null, () => {
			GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
			word.OnShow();
		});

	}



	public void BackFromBonusLevel(Action OnFillBlack = null) {

		BlackRound br = (BlackRound)UIManager.Instance.GetPanel(UiType.blackRound);

		br.Play(() => {
			gamePhase = GamePhase.menu;
			if (OnFillBlack != null) OnFillBlack();
			PlayerManager.Instance.company.isBonusLevel = false;

			UIManager.Instance.GetPanel(UiType.game).gameObject.SetActive(false);
			UIManager.Instance.GetPanel(UiType.gameResult).gameObject.SetActive(false);
			UIManager.Instance.GetPanel(UiType.dailyBonusFailed).gameObject.SetActive(false);
			UIManager.Instance.GetPanel(UiType.dailyBonusConfirm).gameObject.SetActive(false);

			switch (PlayerManager.Instance.company.lastPhase) {
				case GamePhase.menu:
					CameraController.Instance.transform.position = new Vector3(0, 0, -10);
					UIManager.Instance.GetPanel(UiType.menu).gameObject.SetActive(true);
					break;
				case GamePhase.locations:
					CameraController.Instance.transform.position = new Vector3(0, 50, -10);
					UIManager.Instance.GetPanel(UiType.locations).gameObject.SetActive(true);
					break;
				case GamePhase.levels:
					CameraController.Instance.transform.position = new Vector3(0, 75, -10);
					UIManager.Instance.GetPanel(UiType.levels).gameObject.SetActive(true);
					break;
			}

		});

	}

	public void BackFromTakeReward(Action OnHide, Action OnShow) {

		BlackRound br = (BlackRound)UIManager.Instance.GetPanel(UiType.blackRound);

		br.Play(() => {
			OnHide();

			gamePhase = GamePhase.menu;
			UIManager.Instance.GetPanel(UiType.levels).gameObject.SetActive(false);
			CameraController.Instance.transform.position = new Vector3(0, 0, -10);
			UIManager.Instance.GetPanel(UiType.menu).gameObject.SetActive(true);

		}, null, () => {
			OnShow();
		});

	}

	public void TutorualToGame(Action FullHide, Action OnStartHide, Action OnEndHide) {
		BlackRound br = (BlackRound)UIManager.Instance.GetPanel(UiType.blackRound);

		br.Play(() => {
			FullHide();

			CameraController.Instance.transform.position = new Vector3(0, 100, -10);
			UIManager.Instance.GetPanel(UiType.splash).gameObject.SetActive(false);
			UIManager.Instance.GetPanel(UiType.menu).gameObject.SetActive(false);
			UIManager.Instance.GetPanel(UiType.game).gameObject.SetActive(true);
			gamePhase = GamePhase.game;

			GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
			word.GraphicReady();

		}, () => {
			OnStartHide();
		}, () => {
			//word.OnShow();
			OnEndHide();
		});
	}



#if UNITY_IOS
	[DllImport("__Internal")] private static extern void requestRate();

	public static float iOSVersion {
		get {
			// SystemInfo.operatingSystem returns something like iPhone OS 6.1
			//float osVersion = -1f;
			//string versionString = SystemInfo.operatingSystem.Replace("iPhone OS ", "").Trim();
			//osVersion = float.Parse(versionString);


			string versionString = Device.systemVersion;//SystemInfo.operatingSystem.Replace("iPhone OS ", "").Trim();

			bool firstPoint = true;
			string newVersionString = "";
			if (versionString.Count(cw => cw.Equals('.')) < 2)
				newVersionString = versionString;
			else {
				for (int i = 0; i < versionString.Length; i++) {
					if (versionString[i] == '.') {

						if (firstPoint) {
							firstPoint = false;

						} else
							break;

					}
					newVersionString += versionString[i];
				}
			}

			return float.Parse(newVersionString);
		}
	}

#endif

	public void RateUsButton() {

#if UNITY_IOS
		try {
			float iosVersion = iOSVersion;
			if (iosVersion >= 10f) {
				requestRate();
				return;
			} else {
				NPBinding.Utility.RateMyApp.m_settings.DontAskButtonText =
			LanguageManager.GetTranslate("rate.cancel");
				NPBinding.Utility.RateMyApp.m_settings.RateItButtonText =
					LanguageManager.GetTranslate("rate.confirm");
				NPBinding.Utility.RateMyApp.m_settings.RemindMeLaterButtonText =
					LanguageManager.GetTranslate("date.rememberLater");
				NPBinding.Utility.RateMyApp.m_settings.Title =
					LanguageManager.GetTranslate("rate.title");
				NPBinding.Utility.RateMyApp.m_settings.Message =
					LanguageManager.GetTranslate("rate.description");
				NPBinding.Utility.RateMyApp.AskForReviewNow();
			}
		} catch (Exception e) {
			NPBinding.Utility.RateMyApp.m_settings.DontAskButtonText =
		LanguageManager.GetTranslate("rate.cancel");
			NPBinding.Utility.RateMyApp.m_settings.RateItButtonText =
				LanguageManager.GetTranslate("rate.confirm");
			NPBinding.Utility.RateMyApp.m_settings.RemindMeLaterButtonText =
				LanguageManager.GetTranslate("date.rememberLater");
			NPBinding.Utility.RateMyApp.m_settings.Title =
				LanguageManager.GetTranslate("rate.title");
			NPBinding.Utility.RateMyApp.m_settings.Message =
				LanguageManager.GetTranslate("rate.description");
			NPBinding.Utility.RateMyApp.AskForReviewNow();
		}
#elif UNITY_ANDROID

		NPBinding.Utility.RateMyApp.m_settings.DontAskButtonText =
			LanguageManager.GetTranslate("rate.cancel");
		NPBinding.Utility.RateMyApp.m_settings.RateItButtonText =
			LanguageManager.GetTranslate("rate.confirm");
		NPBinding.Utility.RateMyApp.m_settings.RemindMeLaterButtonText =
			LanguageManager.GetTranslate("date.rememberLater");
		NPBinding.Utility.RateMyApp.m_settings.Title =
			LanguageManager.GetTranslate("rate.title");
		NPBinding.Utility.RateMyApp.m_settings.Message =
			LanguageManager.GetTranslate("rate.description");
		NPBinding.Utility.RateMyApp.AskForReviewNow();

#endif

	}


}

public enum GamePhase {
	none,
	splash,
	menu,
	locations,
	levels,
	game
}