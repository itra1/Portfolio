using Game.Game.Settings;
using Game.Providers.Battles.Settings;
using Game.Providers.Profile.Handlers;
using Game.Providers.Ui;
using UnityEngine;
using Zenject;

namespace Game.Providers.Battles.Helpers
{
	public class BattleHelper : IBattleHelper
	{
		private readonly IBattleProvider _provider;
		private readonly SignalBus _signalBus;
		private readonly GameSettings _gameSettings;
		private readonly PlayerResourcesHandler _playerResourcesHandler;
		private readonly IUiProvider _uiProvider;

		public BattleHelper(
			SignalBus signalbus,
			IBattleProvider provider,
			GameSettings gameSettings,
			PlayerResourcesHandler playerResourcesHandler,
			IUiProvider uiProvider
		)
		{
			_provider = provider;
			_signalBus = signalbus;
			_gameSettings = gameSettings;
			_playerResourcesHandler = playerResourcesHandler;
			_uiProvider = uiProvider;
		}

		public void RunDuel()
		{
			_provider.RunDuel();
		}

		public void RunSolo(DuelItemSettings runTournament, RectTransform point)
		{
			//var resourcesHandler = _playerResourcesHandler.GetHandler(runTournament.Fee.Type);
			//if (runTournament.Fee.Value == 0 || resourcesHandler.AddValue(-runTournament.Fee.Value, point))
			//	_signalBus.Fire(new PlayGameSignal(runTournament));
		}

		public void AddBattleResult(BattleResult tournamentResult)
		{

			if (_provider.SaveData.Value.TutorialResult.Exists(x => x.Uuid == tournamentResult.Uuid))
				return;

			if (_provider.SaveData.Value.TutorialResult.Count >= _gameSettings.MaxItemInHistory)
			{
				var removeItem = _provider.SaveData.Value.TutorialResult[^1];
				if (removeItem.ExistsReward)
				{
					var resourcesHandler = _playerResourcesHandler.GetHandler(tournamentResult.Tournament.Reward.Type);
					_ = resourcesHandler.AddValue(tournamentResult.Tournament.Reward.Value, null);
				}
				_ = _provider.SaveData.Value.TutorialResult.Remove(removeItem);
			}

			_provider.SaveData.Value.TutorialResult.Add(tournamentResult);
			_provider.ResultsChangeEmit();
		}

		public void UpdateTournamentResult()
		{
			_ = _provider.SaveData.Save();
		}
	}
}
