using Game.Common.Attributes;

namespace Game.Game.Elements.Bonuses {
	[PrefabName(BonusNames.Apple)]
	public class AppleBonus :Bonus {
		public override string FormationSubType => BonusNames.Apple;
	}
}
