using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EditRun {

  public class LevelDataOrders: ScriptableObject {
    public List<LevelDataOrderStruct> orderLevels = new List<LevelDataOrderStruct>();

#if UNITY_EDITOR

    public void SaveData() {
      LevelDataOrderStruct[] tmpOrder = orderLevels.ToArray();
      for (int i = 0; i < tmpOrder.Length; i++) {
        tmpOrder[i].path = tmpOrder[i].levelObject.name;
      }
      orderLevels = tmpOrder.ToList();
      EditorUtility.SetDirty(this);
    }
#endif

    [System.Serializable]
    public struct LevelDataOrderStruct {
      public string path;
      public LevelData levelObject;
    }

  }

}