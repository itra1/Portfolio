using Game.Components;
using Game.Game.Elements.Interfaces;
using Game.Game.Elements.Weapons.Common;
using UnityEngine;
using Zenject;

namespace Game.Game.Elements.Weapons.Base
{
	public abstract class WeaponSpecial : Weapon
	{
		[SerializeField] private int _destroyKnifeCount = 0;
		[SerializeField] protected int _speedRotation = -1000;

		protected int _currentSlickCount;
		[SerializeField] private TriggerEnter2dComponent _trigger;

		[Inject]
		public void Build()
		{
			_trigger.OnTriggerEnter = TriggerEnter2D;
		}

		protected override void SetForwardVelocity()
		{
			base.SetForwardVelocity();
			_rigibody.rotation += _speedRotation * Time.fixedDeltaTime;
		}

		public override void Shoot()
		{
			base.Shoot();
			_currentSlickCount = _destroyKnifeCount;
		}

		private void TriggerEnter2D(Collider2D collider)
		{
			if (State != WeaponStates.Shoot)
				return;

			if (collider.transform.parent.TryGetComponent<IWeaponCollision>(out var weaponCollision))
			{
				if (_currentSlickCount > 0)
				{
					_currentSlickCount--;
					weaponCollision.KnockOut();
					return;
				}
				else
					FlyOut();
			}
		}
	}
}
