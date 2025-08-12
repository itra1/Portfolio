using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using I2.Loc;
using UnityEngine.EventSystems;
using it.Game.Managers;

namespace it.UI.Settings
{
  public class InputKey : Item, UnityEngine.EventSystems.IPointerDownHandler, IPointerUpHandler, IDragHandler
  {
	 public UnityEngine.Events.UnityAction<InputKey> OnClick;

	 [SerializeField] private TextMeshProUGUI _inputKey;
	 [SerializeField] private Color _waitColor;
	 [SerializeField] private Color _activeColor;
	 public GameKeys.KeyData KeyData { get => _keyData; set => _keyData = value; }

	 private bool _isReady = false;
	 private it.Game.Managers.GameKeys.KeyData _keyData;
	 private bool _isDown;

	 protected override void OnEnable()
	 {

	 }

	 public void SetKey(it.Game.Managers.GameKeys.KeyData keyData)
	 {
		KeyData = keyData;
		_titleUi.text = I2.Loc.LocalizationManager.GetTranslation(KeyData.Title);
		SetKey();
	 }

	 public void OnSelect()
	 {
		Debug.Log("OnSelect");
	 }

	 public void SetWaitKey()
	 {
		_inputKey.text = "Enter Key";
	 }

	 public void SetKey()
	 {
		_inputKey.text = KeyData.ValueName;
		_inputKey.color = _activeColor;
	 }

	 public void OnPointerDown(PointerEventData eventData)
	 {
		_isDown = true;
	 }

	 public void OnPointerUp(PointerEventData eventData)
	 {
		if (!_isDown)
		  return;

		OnClick?.Invoke(this);
	 }

	 public void OnDrag(PointerEventData eventData)
	 {
		_isDown = false;
	 }
  }
}