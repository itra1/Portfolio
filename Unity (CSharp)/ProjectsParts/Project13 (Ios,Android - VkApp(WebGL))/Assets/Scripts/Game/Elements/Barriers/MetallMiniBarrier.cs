using Game.Common.Attributes;

namespace Game.Game.Elements.Barriers {
	[PrefabName(BarrierNames.MetallMini)]
	internal class MetallMiniBarrier :Barrier {
		public override string FormationSubType => BarrierNames.MetallMini;
	}
}
