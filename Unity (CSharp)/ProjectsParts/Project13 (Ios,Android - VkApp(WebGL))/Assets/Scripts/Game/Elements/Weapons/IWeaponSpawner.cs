using Game.Base.AppLaoder;

namespace Game.Game.Elements.Weapons
{
	public interface IWeaponSpawner : IAppLoaderElement
	{
		IWeapon Spawn(string weaponType);
		void AddWeapon(string name, int count);
	}
}