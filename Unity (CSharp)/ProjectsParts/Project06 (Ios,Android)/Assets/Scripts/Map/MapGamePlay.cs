using Game.User;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {

  /// <summary>
  /// Геймплей
  /// </summary>
  public class MapGamePlay: UiDialog {

    public GameObject healthPanel;
    public RectTransform healthValue;
    public Text medicineChestButtonText;
    public GameObject arenaButton;

    private void OnEnable() {
      healthPanel.SetActive(UserManager.gameMode == GameMode.crusade);
      if(UserManager.gameMode == GameMode.crusade)
        healthValue.localScale =
          new Vector3(UserHealth.Instance.Percent, healthValue.localScale.y, healthValue.localScale.z);
      UserMedicineChest.OnChange += MedicineChestAction;

#if OPTION_ARENA
		  arenaButton.SetActive(true);
#else
      arenaButton.SetActive(false);
#endif
    }

    private void OnDisable() {
      UserMedicineChest.OnChange -= MedicineChestAction;
    }

    [ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.KeyDown))]
    private void DownKey(ExEvent.GameEvents.KeyDown keyDown) {
      if (keyDown.keyCode == KeyCode.Escape)
        OptionsButton();
    }

    private void MedicineChestAction(int chestCount, TimeStruct timeStr) {
      if (chestCount < 4) {
        medicineChestButtonText.text = string.Format("{0} - {1}:{2}", chestCount, timeStr.minut, timeStr.second);
      } else {
        medicineChestButtonText.text = chestCount.ToString();
      }
    }

    public void MedicineChestButton() {
      UserManager.Instance.medicineChest.Use();
    }

    public void ZoomPlus() {
      UIController.ClickPlay();
      ExEvent.ScreenEvents.Scroll.Call(1);
    }

    public void ZoomMinus() {
      UIController.ClickPlay();
      ExEvent.ScreenEvents.Scroll.Call(-1);
    }

    public void ProfileButton() {
      UIController.ClickPlay();
      Profile inst = UIController.ShowUi<Profile>();
      inst.gameObject.SetActive(true);
    }

    public void ArenaButton() {
      UIController.ClickPlay();
      LevelInfo pointInfo = new LevelInfo();
      pointInfo.Mode = PointMode.arena;
      pointInfo.Group = 1;
      pointInfo.Level = 1;
      pointInfo.Status = PointSatus.open;

      Briefing instBrief = UIController.ShowUi<Briefing>();
      instBrief.gameObject.SetActive(true);
      instBrief.OnStart = (pointInfoData) => {
        //AudioManager.backMusic.Stop(0.5f, () => {
          UserManager.Instance.ActiveBattleInfo = pointInfoData;
          UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
        //});
      };
      instBrief.FillInfo(pointInfo);
    }

    public void ShopButton() {
      UIController.ClickPlay();
      Shop inst = UIController.ShowUi<Shop>();
      inst.gameObject.SetActive(true);
    }

    public void OptionsButton() {
      UIController.ClickPlay();
      Options panel = UIController.ShowUi<Options>();
      if (panel.gameObject.activeInHierarchy)
        return;
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


  }

}