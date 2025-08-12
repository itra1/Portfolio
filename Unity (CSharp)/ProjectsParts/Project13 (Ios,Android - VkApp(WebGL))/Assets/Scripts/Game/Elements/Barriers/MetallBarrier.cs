using Game.Common.Attributes;

namespace Game.Game.Elements.Barriers {
	[PrefabName(BarrierNames.Metall)]
	public class MetallBarrier :Barrier {
		public override string FormationSubType => BarrierNames.Metall;
	}
}
