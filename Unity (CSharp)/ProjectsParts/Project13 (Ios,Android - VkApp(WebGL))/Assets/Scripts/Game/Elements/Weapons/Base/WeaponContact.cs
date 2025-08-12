using Game.Game.Elements.Boards;
using Game.Game.Elements.Interfaces;
using Game.Game.Elements.Weapons.Common;
using UnityEngine;

namespace Game.Game.Elements.Weapons
{
	public partial class Weapon
	{
		protected void OnCollisionEnter2D(Collision2D collision)
		{
			if (State != WeaponStates.Shoot)
				return;

			if (collision.gameObject.TryGetComponent<IWeaponCollision>(out var weaponCollision))
			{
				weaponCollision.WeaponHit(this);

				if (weaponCollision.GameCollisionType is GameCollisionName.Barrier)
					BarrierCollision2D();

				if (weaponCollision.GameCollisionType is GameCollisionName.Weapon)
					WeaponCollision2D(weaponCollision);

				if (weaponCollision.GameCollisionType is GameCollisionName.Board)
					BoardCollision2D(weaponCollision);

			}
			else if (collision.gameObject.TryGetComponent<IWeaponCollisionBase>(out var commonCollision)
			&& commonCollision.GameCollisionType == GameCollisionName.Common)
			{
				commonCollision.WeaponHit(this);
			}
		}

		protected void OnTriggerEnter2D(Collider2D collision)
		{
			if (State != WeaponStates.Shoot)
				return;

			if (collision.gameObject.TryGetComponent<IWeaponTrigger>(out var trigger))
				trigger.OnWeaponTrigger();

			if (collision.gameObject.TryGetComponent<IWeaponCollision>(out var weaponCollision))
			{
				TriggerWeapon2D(weaponCollision);
			}
		}

		private void BarrierCollision2D()
		{
			WeaponCollision();
		}

		private void WeaponCollision2D(IWeaponCollision weaponCollision)
		{
			WeaponCollision();
		}

		protected virtual void BoardCollision2D(IWeaponCollision weaponCollision)
		{
			if ((weaponCollision as Component).gameObject.TryGetComponent<IBoardHandler>(out var board))
			{
				if (_stickInBoard)
					HitOnBoard(board.Board);
				else
					FlyOut();
				return;
			}
		}

		private void TriggerWeapon2D(IWeaponCollision weaponCollision)
		{
			if (weaponCollision.GameCollisionType is GameCollisionName.Weapon)
			{
				WeaponCollision();
			}
		}
	}
}
