using Game.Game.Elements.Weapons;
using Game.Game.Elements.Weapons.Common;
using UnityEngine;
using Zenject;

namespace Game.Game.Handlers
{
	public class PlayerGameHelper
	{
		private readonly SignalBus _signalBus;
		private readonly WeaponHelper _weaponHelper;
		private readonly IWeaponSpawner _weaponSpawner;
		private Weapon _weapon;

		public PlayerGameHelper(SignalBus signalBus, WeaponHelper weaponHelper, IWeaponSpawner weaponSpawner)
		{
			_signalBus = signalBus;
			_weaponHelper = weaponHelper;
			_weaponSpawner = weaponSpawner;
		}

		public void Shoot()
		{
			if (_weapon != null && _weapon.IsShootReady)
			{
				_weapon.Shoot();
				_weaponHelper.OnShoot(true);
				CreateWeapon();
			}
		}

		public void CreateWeapon(string weaponType = WeaponType.Knife)
		{
			_weapon = _weaponHelper.InitInstance(true, weaponType);
			_weapon.transform.localRotation = Quaternion.identity;
		}

		public void RemoveWeapon()
		{
			_weaponHelper.ClearKnife(true);
		}

		public void ChangeWeapon(string key)
		{
			if (_weapon != null && _weapon.Type == key)
				return;
			RemoveWeapon();
			CreateWeapon(key);

		}

		public void HitDefeat()
		{
			//_gameHandler.PlayerDefeat();
		}
	}
}
