using UnityEngine;
using UnityEngine.Events;

namespace it.Game.Handles
{
  [System.Serializable]
  public class ColliderEvent : UnityEvent<Collider>
  {

  }

  public class TriggerHandler : MonoBehaviourBase
  {
    public ColliderEvent onTriggerEnter = new ColliderEvent();

    public ColliderEvent onTriggerExit = new ColliderEvent();

	 public ColliderEvent onTriggerStay = new ColliderEvent();

	 private void OnTriggerEnter(Collider other) {
		onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit(Collider other) {
      onTriggerExit?.Invoke(other);
    }

    private void OnTriggerStay(Collider other) {
      onTriggerStay?.Invoke(other);
    }

  }
}