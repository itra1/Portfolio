using System.Collections;
using UnityEngine;

namespace Game.Weapon {

  public class Ak47Bullet: Bullet {

    public LayerMask targetMask;
    public LineRenderer linerenderer;

    public override void OnEnable() {
      base.OnEnable();
      linerenderer.positionCount = 0;
    }

    public override void Shot(Vector3 tapStart, Vector3 tapEnd) {
      base.Shot(tapStart, tapEnd);

      RaycastHit2D hit = Physics2D.Linecast(tapStart, (tapEnd - tapStart).normalized * 20, targetMask);

      linerenderer.positionCount = 2;
      linerenderer.SetPosition(0, tapStart);
      linerenderer.SetPosition(1, (tapEnd - tapStart).normalized * 20);

      if (hit.collider != null) {
        OnHit(hit.collider);
      }
      StartCoroutine(Deactive());
    }

    private IEnumerator Deactive() {
      yield return new WaitForSeconds(0.1f);
      gameObject.SetActive(false);
    }

    public override void Update() { }


  }


}