using System;
using ExEvent;
using Game.User;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI {

  /// <summary>
  /// Профиль игрока
  /// </summary>
  public class Profile: UiDialog {

    public Action OnClose;

    public ProfileChangeParametr changeDialog;

    //public Text userName;
    //public Text expirienceCountText;
    //public Text playerLevelText;
    
    //[SerializeField]
    //private TMPro.TextMeshProUGUI balance;

    [SerializeField]
    private TMPro.TextMeshProUGUI userLevel;
    
    public Animation mainAnimation;
    private string profileHideAnim = "ProfileHide";
    private string profileShowAnim = "ProfileShow";
    
    private void OnEnable() {
      mainAnimation.Play(profileShowAnim);

      userLevel.text = UserManager.Instance.UserProgress.PlayerLevel().ToString();

      //expirienceCountText.text = UserManager.Instance.Experience.ToString();
      //playerLevelText.text = UserManager.Instance.UserProgress.PlayerLevel().ToString();
      OnChangeCoins(null);
      //OnChangeEnergy(new ExEvent.UserEvents.OnEnergyLevel(UserManager.Instance.UserProgress.EnergyLevel));
      //OnChangePower(new ExEvent.UserEvents.OnPowerLevel(UserManager.Instance.UserProgress.PowerLevel));
      //HealthiLevelChange(UserManager.Instance.healthLevel.Value);
      
      //UserManager.Instance.healthLevel.OnChangeValue += HealthiLevelChange;

    }

    private void OnDisable() {
      //UserManager.Instance.healthLevel.OnChangeValue -= HealthiLevelChange;

      if (OnClose != null)
        OnClose();
    }

    [ExEvent.ExEventHandler(typeof(UserEvents.OnCoins))]
    private void OnChangeCoins(ExEvent.UserEvents.OnCoins eventData) {
      //balance.text = UserManager.Instance.silverCoins.Value.ToString();
      //Vector2 deltaSize = balance.GetComponent<RectTransform>().sizeDelta;
      //deltaSize.x = balance.preferredWidth;
      //balance.GetComponent<RectTransform>().sizeDelta = deltaSize;
    }

    public void AddStat(Game.User.UserStat stat, int value, System.Action<int> callback) {
      UIController.ClickPlay();
      changeDialog.gameObject.SetActive(true);
      changeDialog.SetData(stat, value);


      changeDialog.OnConfirm = callback;
    }
    
    //public void AddPower() {
    //  UIController.ClickPlay();
    //  changeDialog.gameObject.SetActive(true);
    //  changeDialog.SetData(UserStat.power, ReadyCount(_powerValue));

    //  changeDialog.OnConfirm = (val) => {

    //    if (val == 0)
    //      return;

    //    for (int i = 0; i < val; i++) {
    //      UserManager.Instance.silverCoins.Value -= UserManager.Instance.UserProgress.StatPriceLevel(UserManager.Instance.UserProgress.PowerLevel + 1);
    //      UserManager.Instance.UserProgress.PowerLevel++;
    //    }

    //  };

    //}

    //public void AddHealh() {
    //  UIController.ClickPlay();
    //  changeDialog.gameObject.SetActive(true);
    //  changeDialog.SetData(UserStat.health, ReadyCount(_healthValue));

    //  changeDialog.OnConfirm = (val) => {

    //    if (val == 0)
    //      return;

    //    for (int i = 0; i < val; i++) {
    //      UserManager.Instance.silverCoins.Value -= UserManager.Instance.UserProgress.StatPriceLevel(UserManager.Instance.UserProgress.EnergyLevel + 1);
    //      UserManager.Instance.healthLevel.Value++;
    //    }

    //  };
    //}

    //public void AddEnergy() {
    //  UIController.ClickPlay();
    //  changeDialog.gameObject.SetActive(true);
    //  changeDialog.SetData(UserStat.energy, ReadyCount(_energyValue));

    //  changeDialog.OnConfirm = (val) => {

    //    if (val == 0)
    //      return;

    //    for (int i = 0; i < val; i++) {
    //      UserManager.Instance.silverCoins.Value -= UserManager.Instance.UserProgress.StatPriceLevel(++UserManager.Instance.healthLevel.Value);
    //      UserManager.Instance.UserProgress.EnergyLevel++;
    //    }

    //  };
    //}

    private int ReadyCount(int actualLevel) {
      if (actualLevel == 25)
        return 0;

      int readyCoins = UserManager.Instance.silverCoins.Value;
      int incReady = 0;
      int tmpLevel = actualLevel;
      while (readyCoins > 0) {
        if (tmpLevel == 25)
          break;
        tmpLevel++;
        readyCoins -= UserManager.Instance.UserProgress.StatPriceLevel(tmpLevel);
        if (readyCoins >= 0)
          incReady++;
      }
      return incReady;
    }
    
    public void CloseButton() {
      UIController.ClickPlay();
      mainAnimation.Play(profileHideAnim);
    }

    public void Close() {
      gameObject.SetActive(false);
    }

    
  }

}