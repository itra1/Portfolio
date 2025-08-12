using System;
using Game.Game.Common;
using Game.Game.Elements.Scenes;
using Game.Game.Elements.Weapons;
using Game.Game.Elements.Weapons.Common;
using Game.Game.Settings;
using Game.Providers.Audio.Handlers;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Game.Handlers
{
	public class WeaponHelper
	{
		public UnityAction OnBoardEvent;
		public UnityAction OnBarrierEvent;

		private readonly IWeaponSpawner _spawner;
		private readonly SignalBus _signalBus;
		private readonly GameSession _gameSession;
		private readonly GameSettings _gameSettings;
		//private readonly PointsModificatorHandler _pointsModificatorHandler;
		private readonly AudioHandler _audioHandler;
		private readonly IGameScene _gameScene;

		//private string _lastSoundAttack = "";

		public bool LockedSpawn { get; set; }

		public WeaponHelper(
		SignalBus signalBus,
		IWeaponSpawner spawner,
		IGameScene gameScene,
		GameSession gameSession,
		GameSettings gameSettings,
		//PointsModificatorHandler pointsModificatorHandler,
		AudioHandler audioHandler)
		{
			_spawner = spawner;
			_signalBus = signalBus;
			_gameSession = gameSession;
			_gameSettings = gameSettings;
			//_pointsModificatorHandler = pointsModificatorHandler;
			_audioHandler = audioHandler;
			_gameScene = gameScene;
		}

		public void ClearKnife(bool isPlayer)
		{
			//var player = isPlayer ? _gameSession.Player : _gameSession.Bot;

			//if (player.Weapon != null)
			//{
			//	player.Weapon.gameObject.SetActive(false);
			//	player.Weapon = null;
			//}
		}

		public void OnShoot(bool isPlayer)
		{
			//var player = isPlayer ? _gameSession.Player : _gameSession.Bot;
			//player.Weapon = null;
			//_lastSoundAttack = _audioHandler.PlayRandomClipExclude(SoundNames.Attack, _lastSoundAttack).ClipName;
			//_ = InitInstance(isPlayer);
		}

		public Weapon InitInstance(bool isPlayer, string weaponType = WeaponType.Knife)
		{
			//var player = isPlayer ? _gameSession.Player : _gameSession.Bot;
			//if (player.Weapon != null && player.Weapon.gameObject.activeSelf)
			//	return player.Weapon;

			//if (LockedSpawn)
			//	return null;

			//player.Weapon = _spawner.Spawn(weaponType);
			//player.Weapon.OnSpawned(isPlayer);
			//player.Weapon.gameObject.SetActive(true);

			//player.Weapon.transform.SetParent(isPlayer
			//? _gameScene.SpawnKnifePoint
			//: _gameScene.BotKnifePoint);

			//player.Weapon.transform.localScale = Vector3.one;
			//player.Weapon.transform.localPosition = Vector3.zero;
			//player.Weapon.ShootReady();
			//return player.Weapon;
			return null;
		}

		public void OnBarrier()
		{
			//_pointsModificatorHandler.Reset();
			OnBarrierEvent?.Invoke();
		}

		public void OnBoardAsync(Weapon knife)
		{
			var currentModificator = _gameSession.Modificator;

			var addPoints = Mathf.RoundToInt((float) (Math.Round(currentModificator, 1) * _gameSettings.HitPoints));

			_gameSession.Points += (int) addPoints;
			//_signalBus.Fire(new LevelPointsChangeSignal(_gameSession.Points));

			//_pointsModificatorHandler.Increment();

			//var player = knife.IsPlayer ? _gameSession.Player : _gameSession.Bot;

			//player.SetHit(player.CurrentHit - 1);
			//if (player.CurrentHit == 0)
			//{
			//	_pointsModificatorHandler.StopDelayReset();
			//	_gameSession.ShootReady = false;
			//	await UniTask.Delay(200);
			//	_gameSession.Board.Destroy(knife.IsPlayer);
			//	_gameSession.Board = null;
			//}
			OnBoardEvent?.Invoke();
		}
	}
}
