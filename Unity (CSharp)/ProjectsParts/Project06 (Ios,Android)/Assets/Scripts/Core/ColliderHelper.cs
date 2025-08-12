using UnityEngine;
using System.Collections;

/// <summary>
/// Помошник по работе с коллайдерами
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ColliderHelper : MonoBehaviour {

  public delegate void ColliderEnter(Collider2D colliderTrigger);
  public ColliderEnter OnColliderEnter;
  public ColliderEnter OnColliderStay;

  void OnTriggerEnter2D(Collider2D colliderTrigger) {
    if(OnColliderEnter != null) OnColliderEnter(colliderTrigger);
  }

  void OnTriggerStay2D(Collider2D colliderTrigger) {
    if(OnColliderStay != null) OnColliderStay(colliderTrigger);
  }

}
