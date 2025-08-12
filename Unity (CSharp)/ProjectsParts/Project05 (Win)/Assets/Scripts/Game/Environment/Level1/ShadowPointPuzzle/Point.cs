using UnityEngine;
using UnityEngine.VFX;

namespace it.Game.Environment.Challenges.ShadowPointPuzzle
{
  public class Point: MonoBehaviourBase {
    
    public Vector2Int Coordinate {
      get { return new Vector2Int(Index/4, Index % 4); }
    }

    [SerializeField]
    private int _index;

    public int Index {
      get { return _index; }
    }

	 [SerializeField]
	 private UnityEngine.VFX.VisualEffect _visualEffect;

	 private void OnEnable() {
      ComponentEnable(true);
    }

    private void OnDisable() {
      ComponentEnable(false);
    }

    private void ComponentEnable(bool isEnable) {
      GetComponent<BoxCollider>().enabled = isEnable;
      GetComponent<Animator>().enabled = isEnable;
    }

    private bool _isSelect;
    /// <summary>
    /// Выбран
    /// </summary>
    public bool IsSelect {
      get { return _isSelect; }
    }

	 public VisualEffect VisualEffect { get => _visualEffect; set => _visualEffect = value; }

	 public void Selected(bool isSelect)
    {
      _isSelect = isSelect;
      transform.localScale =  Vector3.one * (isSelect ? 1.3f : 1);
		_visualEffect.SetBool("Select", isSelect);

	 }

  }
}