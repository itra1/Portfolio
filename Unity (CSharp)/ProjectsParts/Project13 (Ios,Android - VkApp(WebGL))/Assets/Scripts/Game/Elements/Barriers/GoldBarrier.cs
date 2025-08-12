using Game.Common.Attributes;

namespace Game.Game.Elements.Barriers {
	[PrefabName(BarrierNames.Gold)]
	public class GoldBarrier :Barrier {
		public override string FormationSubType => BarrierNames.Gold;
	}
}
