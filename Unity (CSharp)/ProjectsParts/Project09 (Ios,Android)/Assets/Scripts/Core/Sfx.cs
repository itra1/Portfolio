using UnityEngine;
using System.Collections;

public class Sfx : MonoBehaviour {

  public float timeLive;

  public void OnEnable() {
    StartCoroutine(Hide(timeLive));
  }

  void Deactive() {
    gameObject.SetActive(false);
  }


  IEnumerator Hide(float time) {
    yield return new WaitForSeconds(time);
    Deactive();
  }

}
