using Game.User;
using UnityEngine;

namespace Game.UI.Battle.UserStat {
  [RequireComponent(typeof(Game.UI.LineProgress))]
  public class LiveLine: MonoBehaviour {

    private Game.UI.LineProgress _lineProgress;

    private Game.UI.LineProgress Line {
      get {
        if (_lineProgress == null)
          _lineProgress = GetComponent<Game.UI.LineProgress>();
        return _lineProgress;
      }
    }

    private void OnEnable() {
      UserHealth.OnChange += HealthChange;
      HealthChange(UserHealth.Instance.Value, UserHealth.Instance.Max);
    }

    private void OnDisable() {
      UserHealth.OnChange -= HealthChange;
    }

    private void HealthChange(float actualValue, float maxValue) {
      Line.Change(actualValue, maxValue);
    }


  }
}