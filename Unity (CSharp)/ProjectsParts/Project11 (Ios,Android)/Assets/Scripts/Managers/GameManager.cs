using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_IOS
using UnityEngine.iOS;
using System.Runtime.InteropServices;
using System.Linq;
#endif
#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor: Editor {
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    if (GUILayout.Button("Удалить соxранения"))
      PlayerPrefs.DeleteAll();

  }
}

#endif

public class GameManager: Singleton<GameManager> {

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

  private bool firstStartInSessin = true;

  private void Start() {

    firstStartInSessin = true;

    FirebaseManager.Instance.LogEvent("start");
    Application.targetFrameRate = 60;

    SceneManager.sceneLoaded += SubscribeSceneLoader;

    if (SceneManager.GetActiveScene().name == "Game") {

      MenuUi menuUi = UIManager.Instance.GetPanel<MenuUi>();

      menuUi.gameObject.SetActive(true);

      if (firstStartInSessin) {
        menuUi.logo.SetActive(false);
      }

      if (firstStartInSessin) {
        UIManager.Instance.GetPanel<SplashUi>().gameObject.SetActive(true);
      }

    }

  }

  private void SubscribeSceneLoader(Scene scene, LoadSceneMode mode) {
    switch (SceneManager.GetActiveScene().name) {
      case "Loader":
      case "Game":

        switch (gamePhase) {
          case GamePhase.menu:
            UIManager.Instance.GetPanel<MenuUi>().gameObject.SetActive(true);
            if (firstStartInSessin) {
              UIManager.Instance.GetPanel<SplashUi>().gameObject.SetActive(true);
            }
            break;
          case GamePhase.game:
            CameraController.Instance.transform.position = new Vector3(0, 100, -10);
            UIManager.Instance.GetPanel<PlayGamePlay>().gameObject.SetActive(true);
            GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
            word.Init();

            if (PlayerPrefs.GetInt("tutor", 0) == 0) {
              UIManager.Instance.GetPanel<TutorialUi>().gameObject.SetActive(true);
              PlayerPrefs.SetInt("tutor", 1);
            }

            break;
        }

        break;
      case "Map":
        UIManager.Instance.GetPanel<MapUI>().gameObject.SetActive(true);
        break;
    }

    StartCoroutine(RateCor());
    StartCoroutine(ShowFullScreen());
  }

  private IEnumerator ShowFullScreen() {
    yield return new WaitForSeconds(4);
    if (PlayerManager.Instance.playCount != 0 && PlayerManager.Instance.playCount != 1 && PlayerManager.Instance.playCount % 3 == 0)
      KingBird.Ads.AdsKingBird.Instance.ShowFullBanner();
  }

  private bool firstOpen = true;

  private IEnumerator RateCor() {
    yield return new WaitForSeconds(4);
    if (PlayerManager.Instance.playCount == 3 && firstOpen) {
      RateUsButton();
      firstOpen = false;
    }
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

    //if (UIManager._instance.GetPanel<ShopUi>().isActiveAndEnabled) return;

    SettingUi set = UIManager._instance.GetPanel<SettingUi>();
    set.gameObject.SetActive(true);
    set.Show();
  }

  public void Shop() {

    //if (UIManager._instance.GetPanel<SettingUi>().isActiveAndEnabled) return;

    ShopUi shop = UIManager._instance.GetPanel<ShopUi>();
    shop.gameObject.SetActive(true);
    shop.Show();
  }

  public void GetBonus() {
    Debug.Log("GetBonus");
  }

  public void AfterSplash() {

    AudioManager.Instance.PlayBackMusic(0);
    ToMenu();

  }

  public void ToMenu() {
    BlackRound br = BlackRound.Instance;
    br.Play(() => {
      UIManager.Instance.GetPanel<SplashUi>().gameObject.SetActive(false);
      gamePhase = GamePhase.menu;

      MenuUi menuUi = UIManager.Instance.GetPanel<MenuUi>();
      UIManager.Instance.Show<MenuUi>();

      if (firstStartInSessin) {
        menuUi.PlayLogo();
        firstStartInSessin = false;
      }

    });
  }

  /// <summary>
  /// Плей
  /// </summary>
  public void MainPlay() {

    if (PlayerManager.Instance.company.companies.Count <= 0)
      return;

    BlackRound br = BlackRound.Instance;
    br.Play(() => {
      gamePhase = GamePhase.levels;
      SceneManager.LoadScene("Map");
      //UIManager.Instance.GetPanel<PlayGamePlay>().gameObject.SetActive(false);
      //CameraController.Instance.transform.position = new Vector3(0, 50, -10);
    });
  }

  /// <summary>
  /// Переход на уровни
  /// </summary>
  /// <param name="OnChange"></param>
  public void ToLevels(Action OnChange) {
    BlackRound br = BlackRound.Instance;
    br.Play(() => {
      OnChange();
      gamePhase = GamePhase.levels;
      CameraController.Instance.transform.position = new Vector3(0, 75, -10);
    });
  }

  public void ToBack() {

    BlackRound br = BlackRound.Instance;

    //if (gamePhase == GamePhase.levels) {

    //  br.Play(() => {

    //    gamePhase = GamePhase.locations;
    //    CameraController.Instance.transform.position = new Vector3(0, 50, -10);
    //    PlayerManager.Instance.Save();
    //  });
    //  return;
    //}

    br.Play(() => {
      gamePhase = GamePhase.menu;
      SceneManager.LoadScene("Game");
      //CameraController.Instance.transform.position = new Vector3(0, 0, -10);
      UIManager.Instance.GetPanel<MenuUi>().gameObject.SetActive(true);
    });
  }

  public void PlayGame(Action OnFillBlack = null) {
    BlackRound br = BlackRound.Instance;

    br.Play(() => {
      SceneManager.LoadScene("Game");
      if (OnFillBlack != null) OnFillBlack();
      gamePhase = GamePhase.game;
    }, () => {
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

    BlackRound br = BlackRound.Instance;

    br.Play(() => {
      if (OnFillBlack != null) OnFillBlack();
      SceneManager.LoadScene("Map");
      gamePhase = GamePhase.levels;
      UIManager.Instance.GetPanel<PlayGamePlay>().gameObject.SetActive(false);
      CameraController.Instance.transform.position = new Vector3(0, 75, -10);
    });
  }

  public void EndGame(BattlePhase phase, GameCompany.Level alvl, GameCompany.Save.Level lvl) {

    if (phase == BattlePhase.win) {
      if (PlayerManager.Instance.company.isBonusLevel) {
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

  private readonly bool _loadDeady = false;

  /// <summary>
  /// Конец обычной игры
  /// </summary>
  private void EndDefaultGame(GameCompany.Level alvl, GameCompany.Save.Level lvl) {

    if (_loadDeady) return;

    GameResultUi gr = UIManager.Instance.GetPanel<GameResultUi>();
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

        if (PlayerManager.Instance.company.actualLevelNum >= 5 && !PlayerManager.Instance.noAds) {

          GoogleAdsMobile.Instance.ShowInterstitialVideo(
            complete: (result) => {
              PlayGame(() => {
                gr.gameObject.SetActive(false);
              });
            }
          );

        } else {
          PlayGame(() => {
            gr.gameObject.SetActive(false);
          });
        }

        return;
      }

      BackToLevels(() => {
        gr.gameObject.SetActive(false);
      });

    };
  }

  private readonly string acceptVideoToken = "";

  public void CheckAndPlayLevel(int levelNum, Action<bool> OnComplete) {

  }

  private void DailyBonusResyme() {
    PlayerManager.Instance.company.BonusResume();
  }

  private void EndBonusGame() {
    DailyBonusUi gr = UIManager.Instance.GetPanel<DailyBonusUi>();
    gr.gameObject.SetActive(true);

    gr.OnBack = () => {
      //gr.gameObject.SetActive(false);
      BackFromBonusLevel();
      return;
    };

  }
  private void EndBonusGameFailed() {
    DailyBonusResultFailed gr = UIManager.Instance.GetPanel<DailyBonusResultFailed>();
    gr.gameObject.SetActive(true);

    gr.OnBack = () => {
      //gr.gameObject.SetActive(false);
      BackFromBonusLevel();
      return;
    };

    gr.OnOk = () => {
      gr.Close();
      DailyBonusResyme();
    };

  }

  public void EndTutorialGame() {
    TutorConfirmUi tutor = UIManager.Instance.GetPanel<TutorConfirmUi>();
    tutor.gameObject.SetActive(true);

    tutor.OnComplete = () => {

      BlackRound br = BlackRound.Instance;

      br.Play(() => {
        Tutorial.Instance.EndTutorial();
        UIManager.Instance.GetPanel<TutorConfirmUi>().gameObject.SetActive(false);
        UIManager.Instance.GetPanel<PlayGamePlay>().gameObject.SetActive(false);
        UIManager.Instance.GetPanel<MenuUi>().gameObject.SetActive(true);
        gamePhase = GamePhase.menu;
        CameraController.Instance.transform.position = new Vector3(0, 0, -10);
      }, null, () => {

        PlayerManager.Instance.company.CheckDownloadIfNeed();

      });
    };
  }

  public void PlayBonusLevel() {

    BlackRound br = BlackRound.Instance;

    br.Play(() => {
      PlayerManager.Instance.company.isBonusLevel = true;
      PlayerManager.Instance.company.lastPhase = gamePhase;
      PlayerManager.Instance.company.lastBonus++;

      UIManager.Instance.GetPanel<BonusLevelUi>().gameObject.SetActive(false);

      switch (gamePhase) {
        case GamePhase.menu:
          UIManager.Instance.GetPanel<MenuUi>().gameObject.SetActive(false);
          break;
        case GamePhase.levels:
          break;
      }

      CameraController.Instance.transform.position = new Vector3(0, 100, -10);

      gamePhase = GamePhase.game;

      GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
      word.Init();

      UIManager.Instance.GetPanel<PlayGamePlay>().gameObject.SetActive(true);

    }, null, () => {
      GameWorld word = (GameWorld)WorldManager.Instance.GetWorld(WorldType.game);
      word.OnShow();
    });

  }

  public void BackFromBonusLevel(Action OnFillBlack = null) {

    BlackRound br = BlackRound.Instance;

    br.Play(() => {
      gamePhase = GamePhase.menu;
      if (OnFillBlack != null) OnFillBlack();
      PlayerManager.Instance.company.isBonusLevel = false;

      UIManager.Instance.GetPanel<PlayGamePlay>().gameObject.SetActive(false);
      UIManager.Instance.GetPanel<GameResultUi>().gameObject.SetActive(false);
      UIManager.Instance.GetPanel<DailyBonusResultFailed>().gameObject.SetActive(false);
      UIManager.Instance.GetPanel<DailyBonusUi>().gameObject.SetActive(false);

      switch (PlayerManager.Instance.company.lastPhase) {
        case GamePhase.menu:
          CameraController.Instance.transform.position = new Vector3(0, 0, -10);
          UIManager.Instance.GetPanel<MenuUi>().gameObject.SetActive(true);
          break;
        case GamePhase.levels:
          CameraController.Instance.transform.position = new Vector3(0, 75, -10);
          break;
      }

    });

  }

  public void BackFromTakeReward(Action OnHide, Action OnShow) {

    BlackRound br = BlackRound.Instance;

    br.Play(() => {
      OnHide();

      gamePhase = GamePhase.menu;
      CameraController.Instance.transform.position = new Vector3(0, 0, -10);
      UIManager.Instance.GetPanel<MenuUi>().gameObject.SetActive(true);

    }, null, () => {
      OnShow();
    });

  }

  public void TutorualToGame(Action FullHide, Action OnStartHide, Action OnEndHide) {
    BlackRound br = BlackRound.Instance;

    br.Play(() => {
      FullHide();

      CameraController.Instance.transform.position = new Vector3(0, 100, -10);
      UIManager.Instance.GetPanel<SplashUi>().gameObject.SetActive(false);
      UIManager.Instance.GetPanel<MenuUi>().gameObject.SetActive(false);
      UIManager.Instance.GetPanel<PlayGamePlay>().gameObject.SetActive(true);
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
  levels,
  game
}