using Game.Game.Elements.Boards;
using Game.Game.Elements.Weapons.Factorys;
using Game.Providers.Battles.Common;
using Game.Providers.Battles.Settings;
using Zenject;

namespace Game.Game.Common
{
	public class GameSession : IGameSession
	{
		private SignalBus _signalBus;
		private readonly BoardFactory _boardFactory;
		private readonly WeaponFactory _weaponFactory;

		public Battle Battle { get; set; }
		public int StageIndex { get; set; }
		public bool IsGame => Battle != null && Battle.IsActiveBattle;
		public float Modificator { get; set; } = 0;
		public int Points { get; set; } = 0;
		public IBoard Board { get; set; }
		public bool IsTutorial { get; set; }
		public BattleResult BattleResult { get; set; }

		public GameSession(
			SignalBus signalBus,
			BoardFactory boardFactory
		)
		{
			_signalBus = signalBus;
			_boardFactory = boardFactory;
		}
	}
}
