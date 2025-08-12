using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDecor: MonoBehaviour {

  private Animator _animator;

  private Animator animator {
    get {
      if (_animator == null)
        _animator = GetComponent<Animator>();
      return _animator;
    }
  }

  public SpanFloat animaWaits;

  private void OnEnable() {
    StartCoroutine(WaitActive());
  }

  private IEnumerator WaitActive() {

    while (true) {

      float waitSeconds = Random.Range(animaWaits.min, animaWaits.max);
      yield return new WaitForSeconds(waitSeconds);

      animator.SetTrigger("anim" + Random.Range(1, 4).ToString());
    }

  }

}
