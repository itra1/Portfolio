using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Game.Elements.Scenes;
using Game.Game.Elements.Weapons;
using Game.Game.Elements.Weapons.Common;
using Game.Providers.Battles.Common;
using Game.Providers.Battles.Components;
using Game.Providers.Ui;
using Game.Providers.Ui.Controllers;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Providers.Battles.Controllers
{
	public class PlayerDuelController
	{
		public UnityAction<int> OnHitEvent;
		public UnityAction OnLossEvent;

		private readonly BattleDuel _battle;
		private BattleDuelState _battleState;

		private IUiProvider _uiProvider;
		private IWeaponSpawner _weaponSpawner;
		private IGameScene _gameScene;
		private GamePlayDuelWindowPresenterController _gamePlayController;
		private string _weaponName = WeaponType.Knife;
		private List<IWeapon> _weaponInBoards = new();

		private IWeapon Weapon
		{
			get => _battle.State.PlayerState.SelectedWeapon;
			set => _battle.State.PlayerState.SelectedWeapon = value;
		}

		public PlayerDuelController(BattleDuel battle)
		{
			_battle = battle;

			_battleState ??= _battle.State;
		}

		[Inject]
		public void Bind(
			IUiProvider uiProvider,
			IWeaponSpawner weaponSpawner,
			IGameScene gameScene
		)
		{
			_uiProvider = uiProvider;
			_weaponSpawner = weaponSpawner;
			_gameScene = gameScene;

			_gamePlayController ??= _uiProvider.GetController<GamePlayDuelWindowPresenterController>();
			_ = BindAsync();
		}
		private async UniTask BindAsync()
		{
			await UniTask.WaitUntil(() => _gamePlayController.Presenter != null);
			_gamePlayController.Presenter.ScreenClickListener.OnScreenClickEvent.AddListener(ScreenClickEvent);
		}

		private void ScreenClickEvent()
		{
			Shoot();
		}

		private void Shoot()
		{
			if (!_battle.State.ShootReady)
				return;

			if (Weapon != null && !Weapon.IsShootReady)
				return;

			Weapon?.Shoot();
			Weapon = null;
			_weaponName = WeaponType.Knife;
			SpawnNewWeapon();
		}

		public void ClearWeaponInBoards()
		{
			foreach (var item in _weaponInBoards)
			{
				item.Remove();
			}
			_weaponInBoards.Clear();
		}

		public void SetWeapon(string weaponType)
		{
			_weaponName = weaponType;
			SpawnNewWeapon();
		}

		public void SpawnNewWeapon()
		{
			if (Weapon != null)
				Weapon.Transform.gameObject.SetActive(false);

			Weapon = _weaponSpawner.Spawn(_weaponName);
			Weapon.Transform.position = _gameScene.PlayerWeaponPoint.position;
			Weapon.SetMode(0);
			Weapon.OnSpawned();
			Weapon.ShootVector = Vector3.up;
			Weapon.OnBoardEvent = WeaponOnBoard;
			//Weapon.OnWeaponRecapturedEvent = WeaponRecaptured;
			Weapon.OnKnockOut = WeaponKnockOut;
			Weapon.OnLossHit = WeaponLossHit;
			Weapon.Transform.gameObject.SetActive(true);
			_ = Weapon.ShootReady();

			var weaponTransform = Weapon.Transform;
			weaponTransform.rotation = Quaternion.identity;
		}

		private void WeaponLossHit(IWeapon weapon)
		{
			ClearWeaponInBoards();
			OnHitEvent?.Invoke(_weaponInBoards.Count);
			OnLossEvent?.Invoke();
		}

		private void WeaponOnBoard(IWeapon weapon)
		{
			if (!_battle.State.ShootReady)
				return;
			_weaponInBoards.Add(weapon);
			OnHitEvent?.Invoke(_weaponInBoards.Count);
		}

		public void RemoveWeapon()
		{
			if (Weapon == null)
				return;
			(Weapon as Component).gameObject.SetActive(false);
			Weapon = null;
		}

		private void WeaponKnockOut(IWeapon weapon)
		{
			_ = _weaponInBoards.Remove(weapon);
			OnHitEvent?.Invoke(_weaponInBoards.Count);
		}
	}
}
