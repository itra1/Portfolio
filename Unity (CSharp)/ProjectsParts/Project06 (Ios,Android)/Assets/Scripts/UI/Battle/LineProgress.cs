using UnityEngine;

namespace Game.UI {

  public class LineProgress: MonoBehaviour {

    private bool _isInitiate;
    private Vector2 _deltaSize;

    [SerializeField]
    private RectTransform _line;
    
    public void Change(float value, float max) {

      if (!_isInitiate) {
        _isInitiate = true;
        _deltaSize = _line.sizeDelta;
      }

      _line.sizeDelta = new Vector2(_deltaSize.x * (value / max), _deltaSize.y);
    }

  }

}
