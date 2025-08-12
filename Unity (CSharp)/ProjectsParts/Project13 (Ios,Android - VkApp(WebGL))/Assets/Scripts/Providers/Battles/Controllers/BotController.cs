using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Game.Elements.Scenes;
using Game.Game.Elements.Weapons;
using Game.Game.Elements.Weapons.Common;
using Game.Game.Settings;
using Game.Providers.Avatars;
using Game.Providers.Battles.Common;
using Game.Providers.Battles.Components;
using Game.Providers.Nicknames;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Providers.Battles.Controllers
{
	public class BotController
	{
		public UnityAction<int> OnHitEvent;
		public UnityAction OnLossEvent;

		private BattleDuel _battle;
		private BattleDuelState _battleState;
		private BattleDuelBotState _botState;

		private GameSettings _gameSettings;
		private INicknamesProvider _nicknamesProvider;
		private IAvatarsProvider _avatarsProvider;
		private IWeaponSpawner _weaponSpawner;
		private IGameScene _gameScene;
		private List<IWeapon> _weaponInBoards = new();

		private CancellationTokenSource _gameTS;

		private IWeapon Weapon
		{
			get => _battle.State.BotState.SelectedWeapon;
			set => _battle.State.BotState.SelectedWeapon = value;
		}

		public BotController(BattleDuel battle)
		{
			_battle = battle;

			_battleState ??= _battle.State;
			_botState ??= _battleState.BotState;
		}

		[Inject]
		private void Bind(
			GameSettings gamesettings,
			INicknamesProvider nicknameProvider,
			IAvatarsProvider avatarProvider,
			IWeaponSpawner weaponSpawner,
			IGameScene gameScene
		)
		{
			_gameSettings = gamesettings;
			_nicknamesProvider = nicknameProvider;
			_avatarsProvider = avatarProvider;
			_weaponSpawner = weaponSpawner;
			_gameScene = gameScene;
		}

		public void StartGame()
		{
			StopGame();
			_gameTS = new();
			_ = GameProcess();
		}

		public void MakeOpponent()
		{
			_botState.Nickname = _nicknamesProvider.GetRandom();
			_botState.Avatar = _gameSettings.BotAvatar;

			CreateWeapon(WeaponType.Knife);
		}

		public void StopGame()
		{
			if (_gameTS != null)
			{
				if (!_gameTS.IsCancellationRequested)
					_gameTS.Cancel();
				_gameTS.Dispose();
				_gameTS = null;
			}
		}

		private async UniTask GameProcess()
		{
			try
			{
				while (_battleState.ShootReady)
				{

					int time = (_botState.ShootCount < 5 ? 400 : (int) UnityEngine.Random.Range(300, 800));
					await UniTask.Delay(time, cancellationToken: _gameTS.Token);
					_botState.ShootCount++;
					_botState.SelectedWeapon.Shoot();
					Weapon.Shoot();
					CreateWeapon();
					await UniTask.Yield(cancellationToken: _gameTS.Token);
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Bot GameProcess {ex.Message} {ex.StackTrace}");
				_ = GameProcess();
			}
		}

		public void CreateWeapon(string weaponType = WeaponType.Knife)
		{
			Weapon = _weaponSpawner.Spawn(weaponType);
			Weapon.Transform.position = _gameScene.OpponentWeaponPoint.position;
			Weapon.SetMode(1);
			Weapon.OnSpawned();
			Weapon.ShootVector = Vector3.down;
			Weapon.OnBoardEvent = WeaponOnBoard;
			Weapon.OnKnockOut = WeaponKnockOut;
			Weapon.OnLossHit = WeaponLossHit;
			Weapon.Transform.gameObject.SetActive(true);
			_ = Weapon.ShootReady();
			Weapon.Transform.localRotation = Quaternion.Euler(0, 0, 180);
		}

		private void WeaponLossHit(IWeapon weapon)
		{
			ClearWeaponInBoards();
			OnHitEvent?.Invoke(_weaponInBoards.Count);
			OnLossEvent?.Invoke();
		}

		public void ClearWeaponInBoards()
		{
			foreach (var item in _weaponInBoards)
			{
				item.Remove();
			}
			_weaponInBoards.Clear();
		}

		private void WeaponKnockOut(IWeapon weapon)
		{
			_ = _weaponInBoards.Remove(weapon);
			OnHitEvent?.Invoke(_weaponInBoards.Count);
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
			(Weapon as Component).gameObject.SetActive(false);
			Weapon = null;
		}

		private void WeaponRecaptured(IWeapon weapon)
		{
			_ = _weaponInBoards.Remove(weapon);
		}
	}
}
