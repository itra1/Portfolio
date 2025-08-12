using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using I2.Loc;
using UnityEngine.EventSystems;

namespace it.UI.Settings
{
  public class ScrollerItem : Item, IDragHandler
  {
	 public UnityEngine.Events.UnityAction<float> OnChangeEvent;

	 [SerializeField] private RectTransform _rectValue;
	 [SerializeField] private TMPro.TextMeshProUGUI _percentValue;
	 public float Value { get => _value; set => _value = value; }

	 private float _deltaWidth;
	 private float _actualWidth;
	 private float _value;

	 protected override void OnEnable()
	 {
		base.OnEnable();

		_actualWidth = GetWight();
		SetValue(_value);
		Confirm();
	 }

	 private float GetWight()
	 {
		if(_deltaWidth == 0)
		  _deltaWidth = _rectValue.GetComponentInParent<RectTransform>().rect.width;
		return _deltaWidth;
	 }

	 public void OnDrag(PointerEventData eventData)
	 {
		_actualWidth += eventData.delta.x;
		_actualWidth = Mathf.Clamp(_actualWidth, 0, GetWight());
		_value = _actualWidth / GetWight();
		Confirm();
	 }

	 public void SetValue(float val)
	 {
		_value = val;
		_actualWidth = GetWight() * _value;
		Confirm();
	 }

	 private void Confirm()
	 {
		OnChangeEvent?.Invoke(_value);
		_rectValue.localScale = new Vector2(_value, 1);
		_percentValue.text = Mathf.Round(_value * 100f) + "%";
	 }
  }
}