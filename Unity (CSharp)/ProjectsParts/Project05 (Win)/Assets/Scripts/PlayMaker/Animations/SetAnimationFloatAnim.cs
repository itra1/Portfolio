using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace it.Game.PlayMaker.ActorController
{
  [HutongGames.PlayMaker.Tooltip("Анимировании изменения float")]
  public class SetAnimationFloatAnim : FsmStateAction
  {
	 public FsmGameObject _gameObject;
	 public FsmFloat _maxValue;
	 public FsmFloat _currentValue;
	 public string _paramName;
	 public float speedChange = 1;
	 private float _actualValue;
	 private Animator _animator;

	 public override void OnEnter()
	 {
		if (_animator == null)
		  _animator = _gameObject.Value.GetComponent<Animator>();
		_actualValue = _animator.GetFloat(_paramName);
	 }

	 public override void OnUpdate()
	 {

		if (_currentValue.Value > _actualValue)
		{
		  _actualValue += _currentValue.Value * speedChange * Time.deltaTime;
		  if (_actualValue > _currentValue.Value)
			 _actualValue = _currentValue.Value;
		}
		else if (_currentValue.Value < _actualValue)
		{
		  _actualValue -= _currentValue.Value * speedChange * Time.deltaTime;
		  if (_actualValue < _currentValue.Value)
			 _actualValue = _currentValue.Value;
		}

		_animator.SetFloat(_paramName, _actualValue/ _maxValue.Value);
	 }

  }
}