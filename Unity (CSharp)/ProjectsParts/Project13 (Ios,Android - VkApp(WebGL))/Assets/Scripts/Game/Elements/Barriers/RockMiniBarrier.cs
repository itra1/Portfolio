using Game.Common.Attributes;

namespace Game.Game.Elements.Barriers {
	[PrefabName(BarrierNames.RockMini)]
	internal class RockMiniBarrier :Barrier {
		public override string FormationSubType => BarrierNames.RockMini;
	}
}
