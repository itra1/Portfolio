using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilites.Geometry;

public class TestCheckCollider : MonoBehaviour
{
  [ContextMenu("Check")]
  public void CheckCollider()
  {
    RaycastHit _ray;
    RaycastExt.SafeRaycast(transform.position + transform.up * 1 + transform.forward * 3, -transform.forward, out _ray, 3);
    Debug.Log(_ray.collider.name);
  }
}
