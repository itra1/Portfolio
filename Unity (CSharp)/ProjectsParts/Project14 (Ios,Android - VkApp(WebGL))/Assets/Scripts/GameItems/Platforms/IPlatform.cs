using UnityEngine.Events;

namespace Scripts.GameItems.Platforms {
	public interface IPlatform {
		public UnityEvent DestroyEvent { get; }
	}
}