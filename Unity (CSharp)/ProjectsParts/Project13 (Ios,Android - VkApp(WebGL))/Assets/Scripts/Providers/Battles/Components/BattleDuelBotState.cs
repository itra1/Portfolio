namespace Game.Providers.Battles.Components
{
	public class BattleDuelBotState : BattleDuelPlayerBaseState
	{
		public string Nickname { get; set; }
		public int ShootCount { get; set; } = 0;
	}
}
