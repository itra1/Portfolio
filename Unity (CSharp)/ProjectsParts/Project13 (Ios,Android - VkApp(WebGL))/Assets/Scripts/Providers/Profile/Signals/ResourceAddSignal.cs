using Game.Common.Signals;
using UnityEngine;

namespace Game.Providers.Profile.Signals {
	public class ResourceAddSignal : ISignal {
		public string Type { get; private set; }
		public float Value { get; private set; }
		public RectTransform SourcePoint { get; private set; }

		public ResourceAddSignal(string type, float value, RectTransform sourcePoint) {
			Type = type;
			Value = value;
			SourcePoint = sourcePoint;
		}
	}
}
