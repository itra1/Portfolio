using UnityEngine;
using UnityEngine.Events;

namespace Game.Components
{
	public class TriggerEnter2dComponent : MonoBehaviour
	{
		public UnityAction<Collider2D> OnTriggerEnter;
		private void OnTriggerEnter2D(Collider2D collision)
		{
			OnTriggerEnter?.Invoke(collision);
		}
	}
}
