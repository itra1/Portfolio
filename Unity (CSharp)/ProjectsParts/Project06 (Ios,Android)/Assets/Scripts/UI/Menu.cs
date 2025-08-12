using System;
using ExEvent;
using Game.User;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI {

  /// <summary>
  /// Основная навигация игры
  /// </summary>
  public class Menu: EventBehaviour {
    private MenuGamePlay gamePlay;

    private void Start() {
      AudioManager.Instance.PlayBackGroundSound("BackGrounds/main_menu");

      MenuGamePlay gamePlay = UIController.ShowUi<MenuGamePlay>();
      gamePlay.gameObject.SetActive(true);
      gamePlay.OnCompany = OnCompany;
      gamePlay.OnSurvival = OnSurvival;
      gamePlay.OnCrusade = OnCrusade;
      gamePlay.OnArena = OnArena;
      gamePlay.OnProfile = OnProfile;
      gamePlay.OnShop = OnShop;
      gamePlay.OnOptions = OnOptions;
      gamePlay.OnExit = OnExit;
      gamePlay.OnReadXml = OnReadXml;
      OpenBlack(null);
    }

    private void OnCompany() {

      if (!UserManager.Instance.CheckLevelComplited(1, 1)) {

        UserManager.Instance.ActiveBattleInfo = LevelsManager.Instance.LevelsList.Find(x => x.Group == 1 && x.Level == 1);

        CloseBlack(() => {
          SceneManager.LoadScene("Battle");
        });
        return;
      }

      CloseBlack(() => {
        SceneManager.LoadScene("Map");
      });
    }

    private void OnSurvival() {
      if (UserManager.Instance.UserProgress.PlayerLevel() >= 0) {

        LevelInfo pointInfo = new LevelInfo();
        pointInfo.Mode = PointMode.survival;
        pointInfo.Group = 1;
        pointInfo.Level = 1;
        pointInfo.Status = PointSatus.open;

        Briefing instBrief = UIController.ShowUi<Briefing>();
        instBrief.gameObject.SetActive(true);
        instBrief.OnStart = (pointInfoData) => {

          CloseBlack(() => {
            UserManager.Instance.ActiveBattleInfo = pointInfoData;
            SceneManager.LoadScene("Battle");
          });
        };
        instBrief.FillInfo(pointInfo);

      } else
        //UIController.instance.ErrorDialogShow("below level 10");
        ShowErrorDialog("below level 10");
    }

    private void OnArena() {

      LevelInfo pointInfo = new LevelInfo();
      pointInfo.Mode = PointMode.arena;
      pointInfo.SetGroup(1);
      pointInfo.SetLevel(1);
      pointInfo.Status = PointSatus.open;

      Briefing instBrief = UIController.ShowUi<Briefing>();
      instBrief.gameObject.SetActive(true);
      instBrief.OnStart = (pointInfoData) => {

        CloseBlack(() => {
          UserManager.Instance.ActiveBattleInfo = pointInfo;
          SceneManager.LoadScene("Battle");
        });
      };
      instBrief.FillInfo(pointInfo);
    }

    [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.KeyDown))]
    private void DownKey(ExEvent.GameEvents.KeyDown keyDown) {
      if (keyDown.keyCode == KeyCode.Escape)
        OnProfile();
    }

    private void OnCrusade() {
      //UIController.instance.ErrorDialogShow("Section in development");
      ShowErrorDialog("Section in development");
    }

    private void ShowErrorDialog(string testMessage) {
      ErrorDialog gui = UIController.ShowUi<ErrorDialog>();
      gui.gameObject.SetActive(true);
      gui.errorText.text = testMessage;
    }

    private void OnProfile() {
      Profile inst = UIController.ShowUi<Profile>();
      if (inst.gameObject.activeInHierarchy)
        return;
      inst.gameObject.SetActive(true);
    }

    private void OnShop() {
      Shop inst = UIController.ShowUi<Shop>();
      inst.gameObject.SetActive(true);
    }

    private void OnOptions() {
      Options panel = UIController.ShowUi<Options>();
      panel.gameObject.SetActive(true);
      panel.OnExit = OnExit;
      panel.OnShare = OnShare;
    }

    /// <summary>
    /// Выход игнорируется в редакторе или веб плеере.
    /// <link>https://docs.unity3d.com/ru/current/ScriptReference/Application.Quit.html</link>
    /// </summary>
    private void OnExit() {

#if UNITY_STANDALONE
		Application.Quit();
#else

#if UNITY_EDITOR
      //UIController.instance.ErrorDialogShow("No work in editor");
#elif UNITY_WEBGL
    //UIController.instance.ErrorDialogShow("No work in web");
#endif

#endif

    }

    private void OnShare() {

#if !UNITY_IOS && !UNITY_ANDROID
		UIController.Instance.MessageDialog("Section in development");
#endif

    }

    private void OnReadXml() {
      XMLParcer.ParsingComplited += ParsingComplited;
      //XMLParcer.Instance.ReadData();
    }

    private void ParsingComplited(object data) {
      Debug.LogError("ParsingComplited 1");
      XMLParcer.ParsingComplited -= ParsingComplited;
      Debug.LogError("ParsingComplited 2");
      UIController.Instance.MessageDialog("Load complited");
      Debug.LogError("ParsingComplited 3");
    }

    private void OpenBlack(Action OnComplited) {
      FillBlack black = UIController.ShowUi<FillBlack>();
      black.gameObject.SetActive(true);
      black.PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.open, () => {
        if (OnComplited != null)
          OnComplited();
        black.gameObject.SetActive(false);
      }
      );

    }


    private void CloseBlack(Action OnComplited) {
      FillBlack black = UIController.ShowUi<FillBlack>();
      black.gameObject.SetActive(true);
      black.PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.close, OnComplited);
    }


  }


}