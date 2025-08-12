using Game.Common.Attributes;

namespace Game.Game.Elements.Barriers {
	[PrefabName(BarrierNames.IceMini)]
	internal class IceMiniBarrier :Barrier {
		public override string FormationSubType => BarrierNames.IceMini;
	}
}
