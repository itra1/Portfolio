using Game.Base;
using Game.Game.Components;
using Game.Game.Elements.Weapons;
using Game.Game.Handlers;
using UnityEngine;
using Zenject;

namespace Game.Game.Elements.Barriers
{
	public class LightBarrier : MonoBehaviour, IInjection
	{
		[SerializeField] private GameWeaponCollision2dHandler _collisionHandler;
		[SerializeField] private Transform _body;
		[SerializeField] private SpriteRenderer _lightRenderer;
		[SerializeField] private AnimationCurve _animation;
		[SerializeField] private float _lightSpeed = 1;

		private float _time;
		private PlayerGameHelper _playerGameHelper;

		[Inject]
		private void Constructor(PlayerGameHelper playerGameHelper)
		{
			_playerGameHelper = playerGameHelper;

			_collisionHandler.OnCollisionEnterHelper.RemoveAllListeners();
			_collisionHandler.OnCollisionEnterHelper.AddListener(Collision2dEvent);
			_collisionHandler.OnWeaponHit.RemoveAllListeners();
			_collisionHandler.OnWeaponHit.AddListener(WeaponHit);
		}

		private void OnEnable()
		{
			_time = 0;
			transform.rotation = Quaternion.identity;
		}

		private void Update()
		{
			UpdateLigth();
		}

		public void WeaponHit(IWeapon weapon)
		{
			weapon.CriticalHit();
		}

		private void Collision2dEvent(Collision2D collision)
		{
		}

		private void UpdateLigth()
		{
			_time += Time.deltaTime * _lightSpeed;
			if (_time >= 1)
				_time -= 1;

			_lightRenderer.color = new Color(1, 1, 1, _animation.Evaluate(_time));
		}
	}
}
