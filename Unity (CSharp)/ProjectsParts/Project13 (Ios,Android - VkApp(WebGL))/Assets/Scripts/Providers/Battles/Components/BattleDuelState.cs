using Game.Game.Elements.Boards;
using Game.Providers.Battles.Interfaces;
using Game.Providers.Timers.Common;

namespace Game.Providers.Battles.Components
{
	public class BattleDuelState
	{
		public int CurrentStage { get; set; }
		public IDuelStageData Stage { get; set; }
		public bool ShootReady { get; set; }
		public IBoard Board { get; set; }
		public BattleDuelPlayerState PlayerState { get; set; } = new();
		public BattleDuelBotState BotState { get; set; } = new();
		public ITimer StageTimer { get; set; }

		public bool IsPlayerWin => PlayerState.WinCount > BotState.WinCount;
	}
}
