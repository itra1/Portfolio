using UnityEngine;
using UnityEngine.Events;

namespace Scripts.GameItems.Platforms {
	public class Platform : MonoBehaviour, IPlatform {
		[SerializeField] protected Transform _meshParent;
		public int Index { get; set; } = 0;

		[HideInInspector] public UnityEvent DestroyEvent { get; set; } = new();
		private PlatformFormations _formation;

		public PlatformFormations Formation => _formation ??= GetComponent<PlatformFormations>();

		public void OnDestroy() {

		}

		public virtual void ResetFormation() {

		}

	}
}
