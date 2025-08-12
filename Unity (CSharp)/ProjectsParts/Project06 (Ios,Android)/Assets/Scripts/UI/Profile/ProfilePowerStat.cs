namespace Game.UI {
  public class ProfilePowerStat: ProfileStatBase {

    private int _value;

    public override void Increment() {
      UIController.ShowUi<Profile>().AddStat(stat, ReadyCount(_value), (val) => {

        if (val == 0)
          return;

        for (int i = 0; i < val; i++) {
          Game.User.UserManager.Instance.silverCoins.Value -= Game.User.UserManager.Instance.UserProgress.StatPriceLevel(Game.User.UserManager.Instance.UserProgress.PowerLevel + 1);
          Game.User.UserManager.Instance.UserProgress.PowerLevel++;
        }

      });
    }

    protected override void Init() {
      SetValue();
    }

    [ExEvent.ExEventHandler(typeof(ExEvent.UserEvents.OnPowerLevel))]
    private void OnChangePower(ExEvent.UserEvents.OnPowerLevel powerLevel) {
      SetValue();
    }

    private void SetValue() {
      _value = Game.User.UserManager.Instance.UserProgress.PowerLevel;
      _levelText.text = _value.ToString();
      _line.SetValue(_value, 25);
      _incrementText.text = ReadyCount(_value).ToString();
    }


  }
}