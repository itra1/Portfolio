using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowController : MonoBehaviour {

  public ShadowBehaviour prefab;                     // Объект тени
  public ShadowBehaviour instancePrefab;                          // Экземпляр тени

  private void OnEnable() {

    if (instancePrefab != null) {
      instancePrefab.matherObject = gameObject.transform;
      return;
    }

    instancePrefab = Instantiate(prefab.gameObject, transform.position, Quaternion.identity).GetComponent<ShadowBehaviour>();
    instancePrefab.matherObject = gameObject.transform;
    instancePrefab.diff = 0.31f;
    instancePrefab.transform.parent = transform;
  }

  //private void OnDisable() {

  //  if (instancePrefab)
  //    Destroy(instancePrefab);
  //}
  
  public void Dead() {
    StartCoroutine(SetDead());
  }
  public IEnumerator SetDead() {
    yield return new WaitForSeconds(0.1f);
    if (instancePrefab)
      instancePrefab.SetDiff(new Vector3(0.5f, 0, 0), new Vector3(0.3f, 0, 0));
    yield return 0;
  }

  public void Fixed(bool flag) {
    if (instancePrefab)
      instancePrefab.fixedsize = flag;
  }

  public void SetDiff(Vector3 newPos, Vector3 newScale) {
    if (instancePrefab)
      instancePrefab.SetDiff(newPos, newScale);
  }

  public void SetDeff() {
    if (instancePrefab)
      instancePrefab.SetDeff();
  }

  public void Show(bool flag) {
    if (instancePrefab)
      instancePrefab.gameObject.SetActive(flag);
  }

}
