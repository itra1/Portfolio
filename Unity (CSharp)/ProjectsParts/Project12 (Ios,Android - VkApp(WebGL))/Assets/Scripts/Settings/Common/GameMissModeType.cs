using StringDrop;

namespace Game.Scripts.Settings.Common
{
	public struct GameMissModeType
	{
		[StringDropItem] public const string Loss = "Loss";
		[StringDropItem] public const string Rollback = "Rollback";
		[StringDropItem] public const string Ignore = "Ignore";
	}
}
