using UnityEngine;
using System.Collections;

namespace it.Game.Utils
{
  /// <summary>
  /// Аниммированное изменение размера по кривой и времени
  /// </summary>
  public class ScalingAnimatedToCurve : MonoBehaviourBase
  {
	 private float _time;
	 private AnimationCurve _animationCurve;
	 private bool _isAnimate;
	 private float _endTime;
	 private UnityEngine.Events.UnityAction _onComplete;
	 private Vector3 _scale;

	 public static void Play(GameObject target, float time, AnimationCurve curve, UnityEngine.Events.UnityAction onComplete)
	 {
		var comonen = target.AddComponent<ScalingAnimatedToCurve>();
		comonen.Play(time, curve, onComplete);
	 }

	 public void Play(float time, AnimationCurve curve, UnityEngine.Events.UnityAction onComplete)
	 {
		this._time = time;
		this._animationCurve = curve;
		_endTime = Time.time + time;
		_isAnimate = true;
		_onComplete = onComplete;
		_scale = transform.localScale;
		transform.localScale = _scale * _animationCurve.Evaluate(0);
	 }

	 private void Update()
	 {
		if (!_isAnimate)
		  return;

		float perc = 1 - (_endTime - Time.time) / _time;

		float val = _animationCurve.Evaluate(perc);
		transform.localScale = _scale * val;

		if (perc >= 1)
		{
		  _onComplete?.Invoke();
		  _isAnimate = false;
		  Destroy(this);
		}


	 }
  }
}