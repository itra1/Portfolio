using Game.Common.Attributes;

namespace Game.Game.Elements.Barriers {
	[PrefabName(BarrierNames.Rock)]
	public class RockBarrier :Barrier {
		public override string FormationSubType => BarrierNames.Rock;
	}
}
