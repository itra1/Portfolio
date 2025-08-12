using Game.Common.Attributes;

namespace Game.Game.Elements.Barriers {
	[PrefabName(BarrierNames.GoldMini)]
	internal class GoldMiniBarrier :Barrier {
		public override string FormationSubType => BarrierNames.GoldMini;
	}
}
