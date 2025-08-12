using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CupidonBallonsHelper))]
public class CupidonBallonsHelperEditor: Editor {

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    if(GUILayout.Button("Destroy")) {
      ((CupidonBallonsHelper)target).BalloneDestroy();
    }
  }

}


#endif


public class CupidonBallonsHelper : MonoBehaviour {

  public System.Action<int> OnDestroy;

  public int number;

  private void OnEnable() {
    GetComponent<CapsuleCollider2D>().enabled = true;
  }

  public void BalloneDestroy() {
    GetComponent<CapsuleCollider2D>().enabled = false;
    if(OnDestroy != null) OnDestroy(number);
  }
  
}
