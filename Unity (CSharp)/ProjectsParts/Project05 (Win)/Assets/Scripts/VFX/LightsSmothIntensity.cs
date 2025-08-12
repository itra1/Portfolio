using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace it.Game.VFX
{
  public class LightsSmothIntensity : MonoBehaviourBase
  {
	 private Light _light;
	 private float _maxIntensity = 0;
	 public void Play(float time, UnityEngine.Events.UnityAction onComplete)
	 {
		_light = GetComponent<Light>();
		_maxIntensity = _light.intensity;
		_light.intensity = 0;
		_light.DOIntensity(_maxIntensity, time).OnComplete(() => { onComplete?.Invoke(); });
	 }
  }
}