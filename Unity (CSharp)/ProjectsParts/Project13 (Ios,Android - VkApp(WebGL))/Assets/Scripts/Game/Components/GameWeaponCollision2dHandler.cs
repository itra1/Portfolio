using Game.Game.Elements.Interfaces;
using Game.Game.Elements.Weapons;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Game.Components
{
	public class GameWeaponCollision2dHandler : MonoBehaviour, IWeaponCollision
	{
		[HideInInspector] public UnityEvent<Collision2D> OnCollisionEnterHelper = new();
		[HideInInspector] public UnityEvent<Weapon> OnWeaponHit = new();
		[HideInInspector] public UnityEvent OnKnockOut = new();

		public string GameCollisionType => GameCollisionName.Barrier;

		public void WeaponHit(Weapon knife) =>
			OnWeaponHit?.Invoke(knife);

		private void OnCollisionEnter2D(Collision2D collision)
		{
			OnCollisionEnterHelper?.Invoke(collision);
		}

		public void KnockOut()
		{
			OnKnockOut?.Invoke();
		}
	}
}
