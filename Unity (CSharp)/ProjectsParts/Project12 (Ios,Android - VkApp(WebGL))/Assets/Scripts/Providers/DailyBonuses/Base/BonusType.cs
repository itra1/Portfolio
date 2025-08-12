using StringDrop;

namespace Game.Scripts.Providers.DailyBonuses.Base
{
	public struct BonusType
	{
		[StringDropItem] public const string HourBonus = "HourBonus";
		[StringDropItem] public const string DayBonus = "DayBonus";
		[StringDropItem] public const string AdsBonus = "AdsBonus";
	}
}
