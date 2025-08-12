namespace Engine.Scripts.Settings.Common
{
	public interface INoteAccuracy
	{
		string Type { get; }
		bool BreakChain { get; }
		float Score { get; }
		float PercentageTheshold { get; }
		bool Active { get; }
		bool IsPerfect { get; }
		bool IsMiss { get; }

		bool IsActualPercentageTheshold(float time);
	}
}