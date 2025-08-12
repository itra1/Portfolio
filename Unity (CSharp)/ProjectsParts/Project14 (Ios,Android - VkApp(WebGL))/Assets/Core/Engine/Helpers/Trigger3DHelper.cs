using UnityEngine;

namespace Core.Engine.Helpers
{
	public class Trigger3DHelper : MonoBehaviour
	{
		public UnityEngine.Events.UnityEvent<Collider> OnEnterTrigger = new();
		public UnityEngine.Events.UnityEvent<Collider> OnExitTrigger = new();

		private Collision _collider;
		private void Awake()
		{
			_collider = GetComponent<Collision>();
		}

		private void OnTriggerEnter(Collider other)
		{
			OnEnterTrigger?.Invoke(other);
		}

		private void OnTriggerExit(Collider other)
		{
			OnExitTrigger?.Invoke(other);
		}

	}
}
