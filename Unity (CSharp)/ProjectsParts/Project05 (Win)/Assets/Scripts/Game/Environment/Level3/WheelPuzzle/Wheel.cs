using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.Challenges.WheelPuzzle
{
  public class Wheel : MonoBehaviourBase
  {
	 /// <summary>
	 /// Значение
	 /// </summary>
	 [SerializeField]
	 private int _value;
	 public int Value { get => _value; }

	 private int _useCount;
	 [SerializeField]
	 private Light _light;

	 private Quaternion _startRotation;

	 private void Start()
	 {
		_startRotation = transform.localRotation;

	 }

	 /// <summary>
	 /// Множитель
	 /// </summary>
	 [SerializeField]
	 private bool _isMultiply;
	 public bool IsMultiply { get => _isMultiply; }

	 public void ResetData()
	 {
		_useCount = 0;
		if(_light != null)
		_light.intensity = 2.5f;
		transform.DOLocalRotate(_startRotation.eulerAngles, 0.3f);
	 }

	 public bool Rotate()
	 {
		if (!_isMultiply && _useCount == 5)
		  return false;

		_useCount++;
		float degree = _isMultiply ? 180f : 180f / 5f;
		StartCoroutine(Rotate(degree, 0.5f));


		//transform.DOLocalRotate((transform.localRotation * Quaternion.Euler(degree, 0, 0)).eulerAngles, 0.5f);

		if (_light != null)
		  DOTween.To(() => _light.intensity, x => _light.intensity = x, 0.5f * (5 - _useCount), 1f);
		return true;
	 }

	 IEnumerator Rotate(float degree, float duration)
	 {
		float time = 0;
		Quaternion startRotate = transform.localRotation;
		while (time < duration)
		{
		  float dur = time / duration;
		  if (dur > 1)
			 dur = 1;
		  transform.localRotation = Quaternion.Slerp(startRotate, (startRotate * Quaternion.Euler(degree, 0, 0)), dur);
		  yield return null;
		  time += Time.deltaTime;
		}
		transform.localRotation = Quaternion.Slerp(startRotate, (startRotate * Quaternion.Euler(degree, 0, 0)), 1);
	 }
  }
}