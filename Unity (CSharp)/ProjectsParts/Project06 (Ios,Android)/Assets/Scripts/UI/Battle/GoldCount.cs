using UnityEngine;

namespace Game.UI.Battle.UserStat {

  public class GoldCount: MonoBehaviour {

    [SerializeField]
    private TMPro.TextMeshProUGUI _text;

    private void OnEnable() {
      BattleManager.OnGoldCoins += Change;
      Change(BattleManager.Instance.goldCoins);
    }

    private void OnDisable() {
      BattleManager.OnGoldCoins -= Change;
    }

    private void Change(int count) {
      _text.text = count.ToString();
    }

  }
}
