using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

  public Transform parent;

  private void LateUpdate() {
    transform.position = new Vector3(parent.transform.position.x, transform.position.y, transform.position.z);
  }

}
