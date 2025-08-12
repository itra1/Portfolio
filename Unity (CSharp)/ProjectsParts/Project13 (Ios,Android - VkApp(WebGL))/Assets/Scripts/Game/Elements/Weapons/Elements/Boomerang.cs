using DG.Tweening;
using Game.Game.Elements.Interfaces;
using Game.Game.Elements.Weapons.Base;
using Game.Game.Elements.Weapons.Common;
using UnityEngine;

namespace Game.Game.Elements.Weapons.Elements
{
	public class Boomerang : WeaponSpecial
	{
		private int _stepMove = 0;
		private float _splineProgress = 0;
		private float _splineLenght = 0;
		private Vector3 _targetMovePosition;
		private Vector3 _moveVector = Vector3.zero;
		private Vector3 _moveDistance = Vector3.zero;
		private Vector3 _moveTarget = Vector3.zero;
		public override string Type => WeaponType.Boomerang;

		public override void Shoot()
		{
			_splineProgress = 0;
			_stepMove = 0;
			base.Shoot();
			_splineLenght = _gameScene.Spline.CalculateLength();
			Unity.Mathematics.float3 positionTarget = _gameScene.Spline.EvaluatePosition(_splineProgress);
			_targetMovePosition = new(positionTarget.x, positionTarget.y, transform.position.z);
			CalcTargetInSpline();
		}
		protected override void SetForwardVelocity()
		{
			switch (_stepMove)
			{
				case 2:

				case 1:
					//_splineProgress += _moveDistance.magnitude / _splineLenght;
					_splineProgress += 1.3f * Time.fixedDeltaTime;
					//AppLog.Log($"_splineProgress {_splineProgress}");

					if (_splineProgress >= 1)
					{
						FlyComplete();
						return;
					}

					CalcTargetInSpline();
					_rigibody.MovePosition(_targetMovePosition);

					break;
				default:
					CalcMovePosition();
					if ((_targetMovePosition - transform.position).sqrMagnitude < (_moveTarget - transform.position).sqrMagnitude)
					{
						_stepMove = 1;
						float distanceInSpline = (_moveTarget - transform.position).magnitude - (_targetMovePosition - transform.position).magnitude;
						_splineProgress += distanceInSpline / _splineLenght;
						//_gameScene.Spline.EvaluateTangent
						CalcTargetInSpline();
						CalcMovePosition();
					}
					_rigibody.velocity = _moveVector;
					break;
			}
			_rigibody.rotation += _speedRotation * Time.fixedDeltaTime;
		}

		protected void FlyComplete()
		{
			State = WeaponStates.Out;
			FreeState();
			transform.SetParent(null);

			Vector2 moveDirection = (_gameScene.PlayerWeaponPoint.position - transform.position).normalized;

			var rotateAngle = -1000;

			float speed = _shootSpeed;

			_ = _renderer.DOColor(new Color(1, 1, 1, 0), 1f)
			.OnUpdate(() =>
			{
				transform.Rotate(0, 0, rotateAngle * Time.fixedDeltaTime);
				transform.position += (Vector3) moveDirection * speed * Time.fixedDeltaTime;
			})
			.OnComplete(() =>
			{
				gameObject.SetActive(false);
			});
		}

		private void CalcTargetInSpline()
		{
			Unity.Mathematics.float3 positionTarget = _gameScene.Spline.EvaluatePosition(_splineProgress);
			_targetMovePosition = new(positionTarget.x, positionTarget.y, transform.position.z);
		}

		private void CalcMovePosition()
		{
			_moveVector = (_targetMovePosition - transform.position).normalized * _shootSpeed;
			_moveDistance = _moveVector * Time.fixedDeltaTime;
			_moveTarget = transform.position + _moveDistance;
		}

		protected override void BoardCollision2D(IWeaponCollision weaponCollision)
		{
		}
	}
}
