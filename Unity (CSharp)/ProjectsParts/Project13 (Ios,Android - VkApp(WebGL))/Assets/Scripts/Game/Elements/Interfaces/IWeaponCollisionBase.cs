using Game.Game.Elements.Weapons;

namespace Game.Game.Elements.Interfaces
{
	public interface IWeaponCollisionBase
	{
		string GameCollisionType { get; }
		void WeaponHit(Weapon knife);
		void KnockOut();
	}
}
