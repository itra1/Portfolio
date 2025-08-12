using Game.Game.Settings;

namespace Game.Providers.Battles.Interfaces
{
	public interface IBoardStageSettings
	{
		int Spikes { get; }
		bool MoveBarrier { get; }
		bool LoseBarrier { get; }
		bool ScrewBarrier { get; }
		public int BoardHits { get; }
		public RotationBoardOptionsStruct BoardRotation { get; }
	}
}
