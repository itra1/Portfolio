using Game.Common.Attributes;

namespace Game.Game.Elements.Bonuses {
	[PrefabName(BonusNames.Gift)]
	public class GiftBonus :Bonus {
		public override string FormationSubType => BonusNames.Gift;
	}
}
