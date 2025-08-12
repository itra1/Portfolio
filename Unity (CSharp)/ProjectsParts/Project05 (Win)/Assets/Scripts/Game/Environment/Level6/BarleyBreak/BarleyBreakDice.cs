using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Game.Handles;
using UnityEngine.VFX;

namespace it.Game.Environment.Level6.BarleyBreak
{
  public class BarleyBreakDice : MonoBehaviourBase
  {
	 private bool _isSelected;
	 public bool IsSelected { get => _isSelected; set => _isSelected = value; }
	 public bool IsActive { get => _isActive; set => _isActive = value; }

	 private bool _isActive;

	 [SerializeField]
	 private VisualEffect _activeEffect;
	 [SerializeField]
	 private VisualEffect _deactiveEffect;

	 public void SetState(bool isSelected = true, bool force = false)
	 {
		if (!force && isSelected == _isSelected)
		  return;
		_isSelected = isSelected;

		if (isSelected)
		{
		  _activeEffect.SendEvent("OnPlay");
		  _deactiveEffect.SendEvent("OnStop");
		}
		else
		{
		  _activeEffect.SendEvent("OnStop");
		  _deactiveEffect.SendEvent("OnPlay");
		}
	 }

	 public void SetActive()
	 {
		SetState(IsSelected, true);
	 }

	 public void SetDeactive()
	 {
		_activeEffect.SendEvent("OnStop");
		_deactiveEffect.SendEvent("OnStop");
	 }

	 public void OnReset()
	 {
		SetState(false, true);
	 }

	 public void Inverce()
	 {
		SetState(!_isSelected);
	 }


  }
}