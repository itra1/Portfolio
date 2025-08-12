using System.Collections.Generic;
using Game.GameItems.Platforms;
using Scripts.GameItems.Platforms;
using UnityEngine;
using Zenject;

namespace Scripts.Common
{
	/// <summary>
	/// Генератор платформ
	/// </summary>
	public class PlatformSpawner :ITickable
	{
		private readonly SignalBus _sibnalBus;
		private readonly SceneComponents _sceneComponents;
		private readonly PlatformFactory _platformFactory;
		private readonly Platform _finishPlatform;
		private readonly Transform _game;
		private float _lastspawn;
		private int _currentIndex;
		private LevelData _levelData;
		private readonly PlatformSettings _platformSettings;
		private PlatformSettings.LevelSpawnSettings _currentSpawn;
		private bool _isNewStage;

		private List<PlatformFormationItem> _lastSpawnItems = new();

		public bool IsGame { get; set; } = false;

		private int PlatformSpawnCoint => _currentSpawn != null ? _currentSpawn.Platforms.Count : _levelData.PlatformCount;

		public PlatformSpawner(SignalBus signalBus, PlatformFactory platformFactory, SceneComponents sceneComponents, PlatformSettings platformSettings)
		{
			_sibnalBus = signalBus;
			_sceneComponents = sceneComponents;
			_platformFactory = platformFactory;

			_finishPlatform = sceneComponents.FinishPlatform;
			_game = sceneComponents.CameraParent.transform;
			_platformSettings = platformSettings;
			_lastspawn = _platformSettings.PlatformDistance;
			HideAllPlatforms();
		}

		public void ResetGame()
		{
			_isNewStage = false;
			_lastspawn = _platformSettings.PlatformDistance;
			_currentIndex = -1;
			_platformFactory.HideAll();
			_finishPlatform.gameObject.SetActive(false);
		}

		public void SetGameData(LevelData ld)
		{
			_levelData = ld;
			_isNewStage = true;
			_lastSpawnItems.Clear();
			_currentSpawn = _platformSettings.LevelSpawn.Find(x => x.Level == _levelData.Level + 1);
			if (_currentSpawn != null)
				_levelData.PlatformCount = _currentSpawn.Platforms.Count;
		}

		public void Tick()
		{
			if (!IsGame)
				return;

			while (_currentIndex < PlatformSpawnCoint)
			{
				_currentIndex++;
				_lastspawn -= _platformSettings.PlatformDistance;

				var isFinish = _currentIndex >= PlatformSpawnCoint;

				var platform = isFinish ? _finishPlatform : _platformFactory.GetInstance();
				platform.gameObject.SetActive(true);
				platform.Index = _currentIndex;
				var targetFinishPosition = new Vector3(0, _lastspawn, 0);
				platform.transform.position = targetFinishPosition;

				if (!isFinish)
				{
					if (_currentSpawn != null)
					{
						string uuid = _currentSpawn.Platforms[_currentIndex].UUID;
						platform.Formation.SetFormation(uuid);
						platform.transform.rotation = _currentSpawn.Platforms[_currentIndex].Rotation;
					}
					else
					{
						var targetFormation = platform.Formation.GetRandomFormation();

						while (_currentIndex == 0 && targetFormation.ExistsFamage)
							targetFormation = platform.Formation.GetRandomFormation();

						if (!_isNewStage)
							targetFormation = platform.Formation.GetFormation(_lastSpawnItems[_currentIndex].UUID);

						platform.Formation.SetFormation(targetFormation);

						if (!_isNewStage)
						{
							platform.Formation.SetRotation(_lastSpawnItems[_currentIndex].Rotation);
						}
						else
						{
							if (_currentIndex == 0)
								platform.Formation.SetRotation(Quaternion.Euler(0, 180, 0));
							else
								platform.Formation.SetRandomRotation();
							_lastSpawnItems.Add(new PlatformFormationItem() { UUID = targetFormation.Uuid, Rotation = platform.transform.rotation });
						}
					}
				}
				else
				{
					_sceneComponents.CameraParent.PositionFinishPlatform = targetFinishPosition;
				}
			}
		}

		private void HideAllPlatforms()
		{
			_finishPlatform.gameObject.SetActive(false);
		}
	}
}
