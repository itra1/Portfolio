using Game.Game.Settings;
using UnityEngine;

namespace Game.Game.Elements.Boards
{
	public class BoardRotation : MonoBehaviour
	{

		[SerializeField] private Transform _body;

		private RotationBoardOptionsStruct _rotationOptions;
		private float _currentSpeed;
		private float _speedSign;
		private float _targetSpeed;
		private float _nextTimeChange;
		private float _deltaSign;
		private float _speedChange;
		private float _speedModificator = 0.7f;

		public RotationBoardOptionsStruct RotationOptions => _rotationOptions;

		public void OnEnable()
		{
			//RandomRotate();
		}

		public void ResetRotate()
		{
			_body.rotation = Quaternion.identity;
		}
		public void RandomRotate()
		{
			_body.Rotate(Vector3.forward, Random.Range(0, 360f));
		}

		public void Update()
		{
			if (_nextTimeChange < Time.realtimeSinceStartup)
				Recalc();

			if (_currentSpeed != _targetSpeed)
			{
				var min = Mathf.Min(_currentSpeed, _targetSpeed);
				var max = Mathf.Max(_currentSpeed, _targetSpeed);
				_currentSpeed = Mathf.Clamp(_currentSpeed + (_speedChange * _deltaSign * Time.deltaTime), min, max);
			}

			_body.Rotate(Vector3.forward, _currentSpeed * Time.deltaTime);
		}

		public void SetRotation(RotationBoardOptionsStruct rotation)
		{
			_rotationOptions = rotation;
			_currentSpeed = Random.Range(_rotationOptions.Speeds.Min, _rotationOptions.Speeds.Max) * _speedModificator;
			_speedSign = _rotationOptions.Signs[Random.Range(0, _rotationOptions.Signs.Length)];
			_currentSpeed *= _speedSign;
			CalcNextTiimeChange();
		}

		private void Recalc()
		{
			if (_rotationOptions.Signs.Length >= 1)
				_speedSign *= -1;
			CalcSpeed();
		}

		private void CalcSpeed()
		{
			_targetSpeed = Random.Range(_rotationOptions.Speeds.Min, _rotationOptions.Speeds.Max) * _speedModificator;
			_targetSpeed *= _speedSign;
			_deltaSign = Mathf.Sign(_targetSpeed - _currentSpeed);
			_speedChange = Random.Range(_rotationOptions.SpeedChange.Min, _rotationOptions.SpeedChange.Max);
			CalcNextTiimeChange();
		}

		private void CalcNextTiimeChange()
		{
			_nextTimeChange = Time.realtimeSinceStartup + Random.Range(_rotationOptions.TimeRotation.Min, _rotationOptions.TimeRotation.Max);
		}
	}
}
