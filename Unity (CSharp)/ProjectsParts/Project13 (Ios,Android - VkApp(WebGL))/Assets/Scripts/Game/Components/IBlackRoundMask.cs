using UnityEngine;

namespace Game.Game.Components {
	public interface IBlackRoundMask {
		public SpriteRenderer Mask { get; }
		public Color BlackColor { get; }
	}
}
