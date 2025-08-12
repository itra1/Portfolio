using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Game.Common;
using Game.Providers.Battles.Common;
using Game.Providers.Battles.Saves;
using Game.Providers.Battles.Settings;
using Game.Providers.Battles.Signals;
using Game.Providers.Saves;
using UnityEngine;
using Zenject;

namespace Game.Providers.Battles
{
	public class BattleProvider : IBattleProvider
	{
		private readonly BattleSettings _settings;
		private readonly DiContainer _container;
		private readonly SignalBus _signalBus;
		private readonly GameSession _gameSession;
		private readonly ISaveProvider _saveGameProvider;
		private BattleSave _save;

		public bool IsLoaded { get; private set; }
		public List<BattleResult> Results => _save.Value.TutorialResult;
		public BattleSave SaveData => _save;
		public BattleSettings Settings => _settings;
		public bool ExistsRewards => Results.Exists(x => x.ExistsReward);

		public BattleProvider(
			DiContainer container,
			SignalBus signalBus,
			GameSession gameSession,
			BattleSettings settings,
			ISaveProvider saveGameProvider
		)
		{
			_container = container;
			_settings = settings;
			_signalBus = signalBus;
			_gameSession = gameSession;
			_saveGameProvider = saveGameProvider;
		}

		public async UniTask FirstLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			_save = _saveGameProvider.GetProperty<BattleSave>();
			for (var i = 0; i < _save.Value.TutorialResult.Count; i++)
			{
				_container.Inject(_save.Value.TutorialResult[i]);
				_save.Value.TutorialResult[i].StartTimerIfNeed();
			}

			await UniTask.Yield();
		}

		public void RunDuel()
		{
			_gameSession.Battle = new BattleDuel(this);
			_container.Inject(_gameSession.Battle);
			_ = _gameSession.Battle.StartGame();
		}

		public void ResultsChangeEmit()
		{
			_ = SaveData.Save();
			_signalBus.Fire<BattleResultChangeSignal>();
		}

		public async UniTask<BattleTypeSettings> GetBattleSettings(string name)
		{
			var resource = await Resources.LoadAsync<BattleTypeSettings>($"{_settings.SettingsPath}/{name}");
			return resource as BattleTypeSettings;
		}
	}
}
