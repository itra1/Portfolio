using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Game.Handles
{
  public class ColorHide : MonoBehaviourBase
  {
	 private bool isActiveHide = false;
	 [ColorUsage(true)]
	 private Color _targetColor;
	 [ColorUsage(true)]
	 private Color _actualColor;
	 [ColorUsage(true)]
	 private Color _targetTransparentColor;
	 [ColorUsage(true)]
	 private Color _startColor;
	 private float _speed;
	 private float _timeActual = 0;
	 private bool _colorForward;
	 private bool _setHide = true;

	 private MeshRenderer[] _itemMeshes;

	 private UnityEngine.Events.UnityAction onComplete;

	 public void StartAnim(Color colorChange, float speed, bool setHide = true, UnityEngine.Events.UnityAction onComplete = null)
	 {
		this._itemMeshes = GetComponentsInChildren<MeshRenderer>();
		if (_itemMeshes.Length == 0)
		{
		  onComplete?.Invoke();
		  Destroy(this);
		  return;
		}

		this._setHide = setHide;
		this.isActiveHide = true;
		this._targetColor = colorChange;
		this._targetTransparentColor = _targetColor;
		this._targetTransparentColor.a = 1;
		_actualColor = Color.black;
		this._speed = speed;
		this._timeActual = 0;
		this._colorForward = true;
		this.onComplete = onComplete;
		this._startColor = Color.black;

		if (!this._setHide)
		{
		  _startColor = _targetColor;
		  _startColor.a = 0;
		  _targetTransparentColor = Color.black;
		  for (int i = 0; i < _itemMeshes.Length; i++)
		  {
			 Material mat = _itemMeshes[i].material;
			 mat.SetColor("_FullEmission", _startColor);
		  }
		  gameObject.SetActive(true);
		}
	 }

	 private void Update()
	 {
		if (!isActiveHide)
		  return;


		_timeActual += Time.deltaTime * _speed;

		if (_colorForward && _timeActual >= 1)
		{
		  _timeActual = 0;
		  _colorForward = false;
		}
		if (!_colorForward && _timeActual >= 1)
		{
		  onComplete?.Invoke();
		  Destroy(this);
		}

		if (_colorForward)
		{
		  _actualColor = Color.Lerp(_startColor, _targetColor, _timeActual);
		}
		else
		{
		  _actualColor = Color.Lerp(_targetColor, _targetTransparentColor, _timeActual);
		}

		for (int i = 0; i < _itemMeshes.Length; i++)
		{
		  Material mat = _itemMeshes[i].material;
		  mat.SetColor("_FullEmission", _actualColor);
		}
	 }
  }
}