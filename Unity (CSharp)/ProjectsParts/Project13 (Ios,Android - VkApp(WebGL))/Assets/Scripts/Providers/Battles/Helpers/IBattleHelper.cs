using Game.Providers.Battles.Settings;
using UnityEngine;

namespace Game.Providers.Battles.Helpers
{
	public interface IBattleHelper
	{
		void AddBattleResult(BattleResult tournamentResult);
		void RunDuel();
		void RunSolo(DuelItemSettings runTournament, RectTransform point);
		void UpdateTournamentResult();
	}
}