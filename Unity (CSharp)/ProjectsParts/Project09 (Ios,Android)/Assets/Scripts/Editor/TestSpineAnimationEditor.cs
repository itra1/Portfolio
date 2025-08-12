using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Spine;
using Spine.Unity;

[CustomEditor(typeof(TestSpineAnimation))]
public class TestSpineAnimationEditor : Editor {
  
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();
    
    EditorGUILayout.BeginHorizontal();

    if(GUILayout.Button("Set Animation")) {
      ((TestSpineAnimation)target).AnimationSet();
    }

    if(GUILayout.Button("Add Animation")) {
      ((TestSpineAnimation)target).AnimationAdd();
    }

    if(GUILayout.Button("Reset Animation")) {
      ((TestSpineAnimation)target).AnimationReset();
    }

    EditorGUILayout.EndHorizontal();

  }

}
