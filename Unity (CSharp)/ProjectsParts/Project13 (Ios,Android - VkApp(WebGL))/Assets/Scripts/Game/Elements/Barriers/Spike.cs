using Game.Game.Components;
using Game.Game.Elements.Weapons;
using UnityEngine;

namespace Game.Game.Elements.Barriers
{
	public class Spike : MonoBehaviour
	{
		[SerializeField] private GameWeaponCollision2dHandler _collisionHandler;

		private void Awake()
		{
			_collisionHandler.OnCollisionEnterHelper.RemoveAllListeners();
			_collisionHandler.OnCollisionEnterHelper.AddListener(Collision2dEvent);
			_collisionHandler.OnWeaponHit.RemoveAllListeners();
			_collisionHandler.OnWeaponHit.AddListener(KnifeHit);
		}

		public void KnifeHit(Weapon weapon)
		{
		}

		private void Collision2dEvent(Collision2D collision)
		{
		}
	}
}
