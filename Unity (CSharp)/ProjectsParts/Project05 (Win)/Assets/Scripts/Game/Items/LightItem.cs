using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using it.Game.Items;

namespace it.Game.Items
{
  public class LightItem : MonoBehaviour, ILightItem
  {

	 [SerializeField]
	 protected GameObject _backLightPrefab;
	 [SerializeField]
	 private bool _lightingReady;

	 private bool _isLighting;
	 private GameObject _backLight;

	 public void OnDrawGizmosSelected()
	 {
		gameObject.layer = LayerMask.NameToLayer("Item");
	 }

	 public void SetLight(bool isLight)
	 {
		if (!_lightingReady)
		  return;
		if (_isLighting == isLight)
		  return;

		_isLighting = isLight;

		// Отключаем подсветку
		if (!isLight)
		{
		  if (_backLight != null)
		  {
			 HideLightObject(false);
		  }
		  return;
		}

		// Включаем подсветку
		if (_backLight == null)
		{
		  if (_backLightPrefab != null)
			 _backLight = Instantiate(_backLightPrefab, transform.position, Quaternion.identity);
		}

		if (_backLight != null)
		  HideLightObject(true);

	 }

	 private void HideLightObject(bool isActive)
	 {
		_backLight.transform.position = transform.position;
		ParticleSystem[] particles = _backLight.GetComponentsInChildren<ParticleSystem>();

		for (int i = 0; i < particles.Length; i++)
		{
		  var emissionModule = particles[i].emission;
		  emissionModule.enabled = isActive;
		}
	 }
  }
}