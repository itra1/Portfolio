using Game.User;
using UnityEngine;

namespace UI.Battle
{
  public class BattleProgress: MonoBehaviour
  {
    [SerializeField]
    private RectTransform line;

    private bool _isActive = false;
    private Vector2 _deltaSize;

    private Vector2 _actualDelta;

    private void Update()
    {
      if (!_isActive)
      {
        _isActive = true;
        _deltaSize = line.sizeDelta;
        _actualDelta.y = _deltaSize.y;
      }

      if (UserManager.Instance.ActiveBattleInfo.Mode != PointMode.survival && UserManager.Instance.ActiveBattleInfo.Mode != PointMode.arena)
      {
        _actualDelta.x = Mathf.Max(0,(1 - BattleManager.Instance.timeBattle / EnemysSpawn.Instance.timeEndGenerate) * _deltaSize.x);
        line.sizeDelta = _actualDelta;
      }
    }
  }
}