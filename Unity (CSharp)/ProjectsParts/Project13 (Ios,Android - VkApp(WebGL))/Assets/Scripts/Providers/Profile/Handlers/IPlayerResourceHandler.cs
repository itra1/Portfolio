using UnityEngine;

namespace Game.Providers.Profile.Handlers {
	public interface IPlayerResourceHandler {

		public string ResourceType { get; }
		public float CurrentValue { get; }
		public string CurrentValueString { get; }

		public bool AddValue(float value, RectTransform point);
	}
}
