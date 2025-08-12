using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Bson;
using DG.Tweening;
using UnityEngine.Video;
using UnityEngine.VFX;

namespace it.Game.Environment.Level5.TrapRoom
{
  public class Piramida : MonoBehaviourBase
  {
	 [SerializeField]
	 private Light _light;

	 private bool _isActive;
	 public bool IsActive { get => _isActive;}

	 [SerializeField]
	 private bool _isCurrect;
	 public bool IsCurrect { get => _isCurrect;}

	 private void Start()
	 {
		DeactivateLight();
		DeactivateSymbol();
	 }

	 public void ActivateSymbol()
	 {
		GetComponentInChildren<VisualEffect>().SendEvent("OnPlay");
	 }

	 public void DeactivateSymbol()
	 {
		GetComponentInChildren<VisualEffect>().SendEvent("OnStop");
	 }

	 public void SetActiveState(bool setActive)
	 {
		if (IsActive == setActive)
		  return;

		_isActive = setActive;

		if (!_isActive)
		{
		  DeactivateLight();
		}
		else
		{
		  ActivateLight();
		}
	 }

	 private void ActivateLight()
	 {
		_light.DOIntensity(8, 0.5f).SetDelay(0.5f);
	 }

	 private void DeactivateLight()
	 {
		_light.intensity = 0;
	 }

  }
}