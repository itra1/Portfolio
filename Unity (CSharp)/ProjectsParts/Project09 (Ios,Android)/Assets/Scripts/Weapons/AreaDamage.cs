using UnityEngine;
using System.Collections.Generic;

public class AreaDamage : MonoBehaviour {
  
  List<GameObject> damageObject = new List<GameObject>();
  
  void OnTriggerEnter2D(Collider2D col) {
    if (damageObject.Exists(x => x == col.gameObject)) return;
    damageObject.Add(col.gameObject);
  }
}
