using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Utils {

  public static class InstantiateUtils {

    public static T GetDisableInstanceFromList<T>(this MonoBehaviour parent, T prefab, List<T> list) where T : Component {
      T comp = list.Find(x => !x.gameObject.activeInHierarchy);

      if (comp == null) {
        GameObject inst = MonoBehaviour.Instantiate(prefab.gameObject, parent.transform);
        comp = inst.GetComponent<T>();
        list.Add(comp);
      }

      return comp;
    }

  }

}