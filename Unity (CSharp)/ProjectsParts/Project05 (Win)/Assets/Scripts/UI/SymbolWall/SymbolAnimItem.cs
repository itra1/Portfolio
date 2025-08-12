using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace it.UI.Symbols
{
  public class SymbolAnimItem : MonoBehaviourBase
  {
	 [SerializeField]
	 private RectTransform _base;
	 [SerializeField]
	 private Image _full;
	 [SerializeField]
	 private Image _backBlank;
	 [SerializeField]
	 private Image _symbol;
	 [SerializeField]
	 private Image _picture;

	 public Image Full { get => _full; set => _full = value; }
	 public RectTransform Base { get => _base; set => _base = value; }
	 public Image BackBlank { get => _backBlank; set => _backBlank = value; }
	 public Image Symbol { get => _symbol; set => _symbol = value; }
	 public Image Picture { get => _picture; set => _picture = value; }

#if UNITY_EDITOR

#endif
  }
}