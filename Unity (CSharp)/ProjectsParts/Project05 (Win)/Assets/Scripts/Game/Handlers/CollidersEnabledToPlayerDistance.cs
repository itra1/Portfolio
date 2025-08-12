using UnityEngine;

namespace it.Game.Handles {

  [RequireComponent(typeof(CheckToPlayerDistance))]
  public class CollidersEnabledToPlayerDistance: MonoBehaviour {

    [SerializeField]
    private bool m_children = false;

    private void Awake() {
      CheckToPlayerDistance cmp = GetComponent<CheckToPlayerDistance>();
      cmp.onPlayerInDistance = () => {
        EnableComponents(true);
      };
      cmp.onPlayerOutDistance = () => {
        EnableComponents(false);
      };
      EnableComponents(false);
    }

    private void EnableComponents(bool isEnables) {

      Collider[] colliders = m_children
        ? GetComponentsInChildren<Collider>()
        : GetComponents<Collider>();
      foreach (var elem in colliders)
        elem.enabled = isEnables;

    }


  }
}