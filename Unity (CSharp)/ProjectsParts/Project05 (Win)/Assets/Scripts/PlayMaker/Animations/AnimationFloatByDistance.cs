using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.ActorController
{
  public class AnimationFloatByDistance : FsmStateAction
  {
	 public FsmOwnerDefault _gameObject;
	 public FsmFloat _distance;
	 public FsmFloat _maxDistance;
	 public FsmFloat _minDistance;
	 public string _paramName;
	 public FsmFloat _maxValue;
	 public FsmFloat _minValue;
	 private Animator _animator;
	 public float _speedChange = 1;
	 private float _currentValue;
	 private float _targetValue;

	 public override void Reset()
	 {
		_maxDistance = 1;
		_maxValue = 1;
		_minValue = 0;
	 }

	 public override void OnEnter()
	 {
		if (_animator == null)
		{
		  _animator = Fsm.GetOwnerDefaultTarget(_gameObject).GetComponent<Animator>();
		}
	 }

	 public override void OnUpdate()
	 {
		if (_distance.Value > _maxDistance.Value)
		  _targetValue = _maxValue.Value;
		else
		  _targetValue = Mathf.Lerp(_minValue.Value, _maxValue.Value, (_distance.Value - _minDistance.Value) / _maxDistance.Value);

		if (_targetValue > _currentValue)
		{
		  _currentValue += _targetValue * _speedChange * Time.deltaTime;
		  if (_currentValue > _targetValue)
			 _currentValue = _targetValue;
		}
		else if (_targetValue < _currentValue)
		{
		  _currentValue -= _targetValue * _speedChange * Time.deltaTime;
		  if (_currentValue < _targetValue)
			 _currentValue = _targetValue;
		}

		_animator.SetFloat(_paramName, _currentValue);
	 }
  }
}