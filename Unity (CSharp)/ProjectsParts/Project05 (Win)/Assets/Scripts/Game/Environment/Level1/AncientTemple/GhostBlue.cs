using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.Level1.AncientTemple
{
  public class GhostBlue : MonoBehaviourBase
  {
	 private ParticleSystem _particleSystem;
	 private Light _light;

	 private void Start()
	 {
		_particleSystem = GetComponentInChildren<ParticleSystem>();
		_light = GetComponentInChildren<Light>(true);
		_particleSystem.gameObject.SetActive(false);
		_light.gameObject.SetActive(false);
		_light.intensity = 0;
	 }


	 public void Visible()
	 {
		_particleSystem.gameObject.SetActive(true);
		_light.gameObject.SetActive(true);
		_light.intensity = 0;
		_light.DOIntensity(4.8f, 0.5f);

	 }
  }
}