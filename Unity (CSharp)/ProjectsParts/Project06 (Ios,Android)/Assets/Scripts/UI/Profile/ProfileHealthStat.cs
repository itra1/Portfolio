using UnityEngine;

namespace Game.UI {
  public class ProfileHealthStat: ProfileStatBase {

    private int _value;

    public override void Increment() {
      UIController.ShowUi<Profile>().AddStat(stat, ReadyCount(_value), (val) => {

        if (val == 0)
          return;

        for (int i = 0; i < val; i++) {
          Game.User.UserManager.Instance.silverCoins.Value -= Game.User.UserManager.Instance.UserProgress.StatPriceLevel(Game.User.UserManager.Instance.healthLevel.Value + 1);
          Game.User.UserManager.Instance.healthLevel.Value++;
        }

      });
    }

    protected override void Init() {
      SetValue();
    }

    [ExEvent.ExEventHandler(typeof(ExEvent.UserEvents.OnHealthLevel))]
    private void OnChangeHealth(ExEvent.UserEvents.OnHealthLevel healthLevel) {
        SetValue();
    }

    private void SetValue() {
      _value = Game.User.UserManager.Instance.healthLevel.Value;
      _levelText.text = _value.ToString();
      _line.SetValue(_value, 25);
      _incrementText.text = ReadyCount(_value).ToString();
    }
  }
}