using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;
using it.Game.Managers;

namespace it.Game.Environment.TimeWhell.Whell
{

  /// <summary>
  /// Колесо времени
  /// </summary>
  public class Whell : MonoBehaviourBase
  {
	 [SerializeField]
	 private Transform _rotateObject;
	 [SerializeField]
	 private Section[] _sections;
	 [SerializeField]
	 private AnimationCurve _curvechengeSpeed;
	 [SerializeField]
	 private float _maxRotateSpeed = 5;
	 [SerializeField]
	 private Material _material;
	 [ColorUsage(false,true)]
	 [SerializeField]
	 public Color _emissionColor;
	 [SerializeField]
	 private ParticleSystem _dirtParticle;
	 [SerializeField]
	 private ParticleSystem _auraParticle;


	 private bool _isRotate;
	 private float _actualSpeed;
	 private int _rotateCircle;
	 private PhaseType _phase;
	 private enum PhaseType
	 {
		start, idle, stop
	 }
	 private void Start()
	 {
		DeactivateAll();
	 }

	 [ContextMenu("Start rotate")]
	 public void StartRotate()
	 {
		if (_isRotate)
		  return;
		_rotateCircle = (int)UnityEngine.Random.Range(5, 8);
		_actualSpeed = 0;
		_isRotate = true;
		_phase = PhaseType.start;
		_dirtParticle.Play();
	 }

	 private void DeactivateAll()
	 {
		for (int i = 0; i  < _sections.Length; i++)
		{
		  var emission = _sections[i].particles.emission;
		  emission.enabled = false;
		  _sections[i].particles.Stop();
		}
		_material.SetColor("_EmissionColor2", Color.black);
	 }
	 [ContextMenu("Activate 2")]
	 public void Activate2()
	 {
		DeactivateAll();
		Activate(2);
	 }
	 [ContextMenu("Activate 3")]
	 public void Activate3()
	 {
		DeactivateAll();
		Activate(3);
	 }
	 [ContextMenu("Activate 4")]
	 public void Activate4()
	 {
		DeactivateAll();
		Activate(4);
	 }
	 [ContextMenu("Activate 5")]
	 public void Activate5()
	 {
		DeactivateAll();
		Activate(5);
	 }
	 [ContextMenu("Activate 6")]
	 public void Activate6()
	 {
		DeactivateAll();
		Activate(6);
	 }
	 [ContextMenu("Activate 7")]
	 public void Activate7()
	 {
		DeactivateAll();
		Activate(8);
	 }
	 public void ActivateTrigger()
	 {
		StartRotate();
	 }

	 public void Activate(int index)
	 {

		for (int i = 0; i < _sections.Length; i++)
		{
		  if(index == _sections[i].levelNum)
		  {
			 var emission = _sections[i].particles.emission;
			 emission.enabled = true;
			 _sections[i].particles.Play();
			 _material.SetTexture("_Emission2", _sections[i].emissionTexture);
			 DOTween.To(() => _material.GetColor("_EmissionColor2"), (x) => _material.SetColor("_EmissionColor2", x), _emissionColor, 3f);
		  }
		}
	 }

	 private void Update()
	 {
		if (!_isRotate)
		  return;

		float currentRotate = _rotateObject.localEulerAngles.z;

		if (_phase == PhaseType.start)
		{

		  _actualSpeed += (_maxRotateSpeed / 5) * Time.deltaTime;
		  if (_actualSpeed >= _maxRotateSpeed)
		  {
			 _actualSpeed = _maxRotateSpeed;
			 _phase = PhaseType.idle;
		  }
		}
		if (_phase == PhaseType.idle)
		{
		  if (_rotateCircle <= 1)
		  {
			 _phase = PhaseType.stop;
		  }
		}
		if (_phase == PhaseType.stop)
		{
		  float percent = currentRotate / 360;
		  _actualSpeed = Mathf.Lerp(0,_maxRotateSpeed, _curvechengeSpeed.Evaluate(percent));
		}

		_rotateObject.Rotate(0, 0, _actualSpeed * Time.deltaTime, Space.Self);
		Debug.Log(_rotateObject.localEulerAngles);

		if (currentRotate > 300 && _rotateObject.localEulerAngles.z < 100)
		{
		  _rotateCircle--;
		  Debug.Log("Circle: " + _rotateCircle);
		  if(_phase == PhaseType.stop)
		  {
			 _rotateObject.localRotation = Quaternion.Euler(0,0,0);
			 _isRotate = false;
			 RotateComplete();
		  }
		}

	 }
	 [ContextMenu("RotateComplete")]
	 private void RotateComplete()
	 {
		//Activate(GameManager.Instance.TargetNewLevel);
		Activate(3);
		_auraParticle.Play();
		DOVirtual.DelayedCall(5, () =>
		{
		  GameManager.Instance.QuitGame();
		  //GameManager.Instance.NextLevel(GameManager.Instance.TargetNewLevel);
		}, false);
	 }

	 [System.Serializable]
	 public struct Section
	 {
		public int levelNum;
		public Texture emissionTexture;
		public ParticleSystem particles;

	 }

  }

}