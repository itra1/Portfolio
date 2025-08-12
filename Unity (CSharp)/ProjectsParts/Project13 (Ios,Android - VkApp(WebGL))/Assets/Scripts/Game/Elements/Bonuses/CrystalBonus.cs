using Game.Common.Attributes;

namespace Game.Game.Elements.Bonuses {
	[PrefabName(BonusNames.Crystal)]
	public class CrystalBonus :Bonus {
		public override string FormationSubType => BonusNames.Crystal;
	}
}
