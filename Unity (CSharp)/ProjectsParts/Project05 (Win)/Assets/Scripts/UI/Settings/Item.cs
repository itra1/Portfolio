using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using I2.Loc;

namespace it.UI.Settings
{
  public class Item : MonoBehaviourBase
  {
	 public UnityEngine.Events.UnityAction<Item> _onFocus;
	 [SerializeField]
	 protected TextMeshProUGUI _titleUi;
	 [SerializeField]
	 private Color _selectColorText;
	 [SerializeField]
	 private RectTransform _focusRect;
	 private bool _isFocus;

	 public bool IsFocus { get => _isFocus; set => _isFocus = value; }

	 protected virtual void OnEnable()
	 {

	 }
	 protected virtual void OnDisable()
	 {

	 }

	 public void Focus()
	 {
		_onFocus?.Invoke(this);
	 }

	 public void SetFocus(bool isFocus)
	 {
		_focusRect.gameObject.SetActive(isFocus);
		_titleUi.color = isFocus ? _selectColorText : Color.white;
		IsFocus = isFocus;
	 }


  }
}