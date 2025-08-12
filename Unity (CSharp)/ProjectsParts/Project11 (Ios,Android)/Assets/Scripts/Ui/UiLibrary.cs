using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(UiLibrary))]
public class UiLibraryEditor: Editor {

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    if (GUILayout.Button("Save")) {
      EditorUtility.SetDirty(target);
    }

  }

  [MenuItem("Assets/Create/Custom/UiLibrary")]
  public static void CreateObject() {
    EditorUtil.CreateAsset<UiLibrary>("Library util", "Library util");
  }

}

#endif

public class UiLibrary : ScriptableObject {
  public List<UiPanel> uiList;

}
