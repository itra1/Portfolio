using Game.Common.Attributes;

namespace Game.Game.Elements.Barriers {
	[PrefabName(BarrierNames.Ice)]
	public class IceBarrier :Barrier {
		public override string FormationSubType => BarrierNames.Ice;
	}
}
