using UnityEngine;
using UnityEngine.Events;

namespace Game.UI {


  public abstract class ProfileStatBase: ExEvent.EventBehaviour {

    [SerializeField]
    protected Game.User.UserStat stat;
    [SerializeField]
    protected TMPro.TextMeshProUGUI _levelText;
    [SerializeField]
    protected TMPro.TextMeshProUGUI _incrementText;
    [SerializeField]
    protected LineValue _line;

    private void OnEnable()
    {
      Init();
    }

    protected abstract void Init();

    public abstract void Increment();


    protected int ReadyCount(int actualLevel) {
      if (actualLevel == 25)
        return 0;

      int readyCoins = Game.User.UserManager.Instance.silverCoins.Value;
      int incReady = 0;
      int tmpLevel = actualLevel;
      while (readyCoins > 0) {
        if (tmpLevel == 25)
          break;
        tmpLevel++;
        readyCoins -= Game.User.UserManager.Instance.UserProgress.StatPriceLevel(tmpLevel);
        if (readyCoins >= 0)
          incReady++;
      }
      return incReady;
    }

  }

}
