using System.Collections.Generic;

namespace Game.Game.Elements.Weapons.Factorys
{
	public interface IWeaponFactory
	{
		List<Weapon> PrefabsList { get; }

		IWeapon GetInstance(string weaponType);
	}
}