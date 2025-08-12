using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{

  public SpanFloat roadSize;                    // Ширина дорожки

  private void OnDrawGizmos() {
    Gizmos.color = Color.yellow;
    Gizmos.DrawLine(new Vector3(-12, roadSize.max, 0), new Vector3(12, roadSize.max, 0));
    Gizmos.DrawLine(new Vector3(-12, roadSize.min, 0), new Vector3(12, roadSize.min, 0));
  }
}
