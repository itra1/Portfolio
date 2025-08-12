using Game.Game.Components;
using Game.Game.Elements.Weapons;
using UnityEngine;

namespace Game.Game.Elements.Barriers
{
	public class Screw : MonoBehaviour
	{
		[SerializeField] private Transform _body;
		[SerializeField] private float _rotationSpeed;
		[SerializeField] private float _rotationBodySpeed;
		[SerializeField] private GameWeaponCollision2dHandler _collisionHandler;

		private void Awake()
		{
			_collisionHandler.OnCollisionEnterHelper.RemoveAllListeners();
			_collisionHandler.OnCollisionEnterHelper.AddListener(Collision2dEvent);
			_collisionHandler.OnWeaponHit.RemoveAllListeners();
			_collisionHandler.OnWeaponHit.AddListener(KnifeHit);
		}

		private void Update()
		{
			Rotation();
			BodyRotation();
		}

		public void KnifeHit(Weapon weapon)
		{
		}

		private void Collision2dEvent(Collision2D collision)
		{
		}

		private void BodyRotation()
		{
			_body.Rotate(Vector3.forward, _rotationBodySpeed * Time.deltaTime);
		}

		private void Rotation()
		{
			transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);
		}
	}
}
