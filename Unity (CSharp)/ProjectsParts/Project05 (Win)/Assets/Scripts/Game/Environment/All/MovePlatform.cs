using UnityEngine;
using System.Collections;

#if UNITY_EDITOR

#endif

namespace it.Game.Environment
{
  public class MovePlatform : MonoBehaviourBase
  {
	 [SerializeField]
	 private bool _active = true;
	 [SerializeField]
	 private bool _fromStart = true;
	 [SerializeField]
	 private Transform _startPosition;
	 [SerializeField]
	 private Transform _endPosition;
	 [SerializeField]
	 private Transform _platform;
	 [SerializeField]
	 private AnimationCurve _moveCorve;
	 [SerializeField]
	 private float _timeMove = 3;
	 [SerializeField]
	 private float _waitOnPosition = 1;
	 private bool _forvard = true;
	 private float _fullDistance;
	 private float _nextTimeMove;

	 private Vector3 _lastPosition;

	 private float _timeStartMove;

	 private bool _playerOnPlatform;

	 //private Rigidbody _rb;

	 private void Start()
	 {
		if (_active)
		  SetActivePlatforms();
	 }

	 public void Activate()
	 {
		if (_active)
		  return;

		_active = true;
		SetActivePlatforms();
	 }

	 private void SetActivePlatforms()
	 {
		_platform.transform.position = _fromStart ? _startPosition.position : _endPosition.position;
		_forvard = _fromStart;
		_fullDistance = (_startPosition.position - _endPosition.position).magnitude;
		_timeStartMove = 0;
		//_rb = _platform.GetComponent<Rigidbody>();
		//if (_rb == null)
		//  _rb = _platform.gameObject.AddComponent<Rigidbody>();
		//_rb.interpolation = RigidbodyInterpolation.Interpolate;
		//_rb.isKinematic = true;
		//_rb.useGravity = false;
	 }

	 Transform Target => _forvard ? _endPosition : _startPosition;
	 Transform Source => _forvard ? _startPosition : _endPosition;
	 Vector3 Direction => _forvard ? (_endPosition.position- _startPosition.position).normalized : (_startPosition.position- _endPosition.position).normalized;

	 private void Update()
	 {
		if (!_active)
		  return;
		if (_nextTimeMove > Time.time)
		  return;

		_timeStartMove += Time.deltaTime;
		float perc = _timeStartMove / _timeMove;
		perc = Mathf.Clamp(perc, 0, 1);
		Vector3 newPosition = Source.position + Direction * _fullDistance* _moveCorve.Evaluate(perc);

		if (perc == 1)
		{
		  _platform.position = Target.position;
		  _platform.rotation = Target.rotation;
		  //_rb.MovePosition(Target.position);
		  //_rb.MoveRotation(Target.rotation);
		  _forvard = !_forvard;
		  _nextTimeMove = Time.time + _waitOnPosition;
		  _timeStartMove = 0;
		}
		else
		{
		  Vector3 befposition = _platform.position;
		  _platform.position = newPosition;
		  _platform.rotation = Quaternion.Lerp(Source.rotation, Target.rotation, _moveCorve.Evaluate(perc));
		  //_rb.MovePosition(newPosition);
		  //_rb.MoveRotation(Quaternion.Lerp(Source.rotation, Target.rotation, _moveCorve.Evaluate(perc)));
		}

	 }

	 private void LateUpdate()
	 {
		if (_playerOnPlatform)
		{
		  CameraBehaviour.Instance.CameraController.Transform.position += _platform.position - _lastPosition;
		}
		_lastPosition = _platform.position;
	 }

	 /// <summary>
	 /// Ишрок на латформа
	 /// </summary>
	 public void PlayerOnPlatform()
	 {
		_playerOnPlatform = true;
	 }
	 /// <summary>
	 /// Игрок сошел с платформы
	 /// </summary>
	 public void PlayerOutPlatform()
	 {
		_playerOnPlatform = false;
	 }


#if UNITY_EDITOR

	 public Mesh _meshDrawner;

	 private void OnDrawGizmos()
	 {
		if (_startPosition == null || _endPosition == null)
		  return;

		if (_meshDrawner == null)
		  _meshDrawner = _platform.GetComponentInChildren<MeshFilter>().mesh;

		if (_meshDrawner == null)
		  return;

		Gizmos.color = Color.blue;
		Gizmos.DrawLine(_startPosition.position, _endPosition.position);
		Gizmos.DrawWireMesh(_meshDrawner, _startPosition.position, _startPosition.rotation, _platform.localScale);
		Gizmos.color = Color.red;
		Gizmos.DrawWireMesh(_meshDrawner, _endPosition.position, _endPosition.rotation, _platform.localScale);
	 }

#endif

  }
}