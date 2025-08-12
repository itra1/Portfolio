using Game.Game.Elements.Interfaces;
using Game.Game.Elements.Weapons;
using UnityEngine;

namespace Game.Game.Elements.Common
{
	public class DespawnCollider : MonoBehaviour, IWeaponCollisionBase
	{
		public string GameCollisionType => GameCollisionName.Common;

		public void WeaponHit(Weapon weapon)
		{
			weapon.gameObject.SetActive(false);
		}

		public void KnockOut()
		{
		}
	}
}
