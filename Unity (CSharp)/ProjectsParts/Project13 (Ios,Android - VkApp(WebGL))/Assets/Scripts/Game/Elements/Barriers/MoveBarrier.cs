using Game.Game.Components;
using Game.Game.Elements.Weapons;
using UnityEngine;

namespace Game.Game.Elements.Barriers
{
	public class MoveBarrier : MonoBehaviour
	{
		[SerializeField] private GameWeaponCollision2dHandler _collisionHandler;
		[SerializeField] private float _rotationSpeed;

		private void Awake()
		{
			_collisionHandler.OnCollisionEnterHelper.RemoveAllListeners();
			_collisionHandler.OnCollisionEnterHelper.AddListener(Collision2dEvent);
			_collisionHandler.OnWeaponHit.RemoveAllListeners();
			_collisionHandler.OnWeaponHit.AddListener(KnifeHit);
		}

		public void KnifeHit(IWeapon weapon)
		{
		}

		private void Collision2dEvent(Collision2D collision)
		{
		}

		private void Update()
		{
			Rotation();
		}

		private void Rotation()
		{
			transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
		}
	}
}
