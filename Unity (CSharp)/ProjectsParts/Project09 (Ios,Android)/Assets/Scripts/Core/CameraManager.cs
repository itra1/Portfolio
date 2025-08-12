using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CameraManager))]
public class CameraManagerEditor : Editor {
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();
    if(GUILayout.Button("Vibration")) {
      ((CameraManager)target).StartVibration();
    }
  }
}

#endif

public class CameraManager : MonoBehaviour {

  public static CameraManager instance;

  void Start() {
    instance = this;
  }

  Coroutine vibrate;

  public void StartVibration() {
    if(vibrate != null) StopCoroutine(vibrate);
    vibrate = StartCoroutine(Vibration());
  }

  IEnumerator Vibration() {
    int vibrcount = Random.Range(4,8);

    while(vibrcount > 0) {
      transform.localPosition = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
      vibrcount--;
      yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));
    }

    transform.localPosition = Vector3.zero;

  }

}
