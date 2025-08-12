using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Handles {
  public class EventHandler: MonoBehaviourBase {
    [SerializeField]
    private UnityEngine.Events.UnityEvent<string> onEvent;

    private void OnDrawGizmosSelected() {

      if (onEvent.GetPersistentEventCount() == 0)
        return;

      for(int i = 0; i < onEvent.GetPersistentEventCount(); i++) {
        if (onEvent.GetPersistentTarget(i) != null) {
          Vector3 target = (onEvent.GetPersistentTarget(i) as MonoBehaviour).transform.position;
          Game.Utils.DrawArrow.GizmoToTarget(transform.position, target, Color.magenta, 1);
          Gizmos.DrawSphere(target, 0.3f);
        }
      }

    }

    public void Emit(string eventName) {
      onEvent?.Invoke(eventName);
    }

  }
}