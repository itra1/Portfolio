using System.Collections;
using UnityEngine;

public abstract class MonoBehaviourBase: MonoBehaviour {

  public void InvokeCustom(float time, System.Action function) {
    StartCoroutine(InvokeCoroutine(time, function));
  }

  public IEnumerator InvokeCoroutine(float time, System.Action function) {
    yield return new WaitForSeconds(time);
    function?.Invoke();
  }

}
