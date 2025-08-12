using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Base.AppLaoder;
using Game.Providers.Battles.Saves;
using Game.Providers.Battles.Settings;

namespace Game.Providers.Battles
{
	public interface IBattleProvider : IAppLoaderElement
	{
		BattleSettings Settings { get; }
		BattleSave SaveData { get; }
		bool ExistsRewards { get; }
		List<BattleResult> Results { get; }

		UniTask<BattleTypeSettings> GetBattleSettings(string name);
		void ResultsChangeEmit();
		void RunDuel();
	}
}