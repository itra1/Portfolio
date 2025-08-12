using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UiLibrary : ScriptableObject {

#if UNITY_EDITOR

  [MenuItem("Assets/Custom/UiLibrary")]
  public static void CreateInstance() {

    EditorUtil.CreateAsset<UiLibrary>("UiLibrary", "UiLibrary");

  }


  [ContextMenu("Find UiLibrary")]
  public void FindObjects() {

    dialogLibrary = EditorUtil.GetAllPrefabsOfType<UiDialog>();
    EditorUtility.SetDirty(this);
  }

#endif

  [SerializeField]
  private List<UiDialog> dialogLibrary;
  public List<UiDialog> DialogLibrary { get { return dialogLibrary; } }

}
