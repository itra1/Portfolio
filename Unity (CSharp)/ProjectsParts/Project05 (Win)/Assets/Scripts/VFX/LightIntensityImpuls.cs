using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace it.Game.VFX
{
  [ExecuteInEditMode]
  public class LightIntensityImpuls : MonoBehaviourBase
  {
	 private Light _m_light;
	 [SerializeField]
	 private RangeFloat _values = new RangeFloat();
	 [SerializeField]
	 private float _rate = .3f;
	 private float _lastTime = 0;

	 private bool _isPlay;

	 private float GetTime()
	 {
		return Application.isPlaying ? Time.time : Time.realtimeSinceStartup;
	 }

	 public void Play(float rate, RangeFloat span)
	 {
		_lastTime = GetTime();
		_m_light = GetComponent<Light>();
		_rate = rate;
		_values = span;
		_isPlay = true;
	 }
	 [ContextMenu("Play Imidiate")]
	 public void PlayImidiate()
	 {
		_lastTime = GetTime();
		_m_light = GetComponent<Light>();
		_isPlay = true;
	 }

	 [ContextMenu("Stop")]
	 public void Stop()
	 {
		_isPlay = false;
	 }

	 private void Update()
	 {
		if (!_isPlay)
		  return;

		if (_lastTime > GetTime())
		  return;

		_lastTime = GetTime() + (1 / _rate);
		_m_light.intensity = UnityEngine.Random.Range(_values.Min, _values.Max);
		//m_light.DOIntensity(UnityEngine.Random.Range(m_values.x,m_values.y),m_duration).OnComplete(LightChange);
	 }

  }
}
