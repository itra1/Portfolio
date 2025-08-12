using System.Collections.Generic;
using UnityEngine;

public class DrawLine: MonoBehaviour {

  [ContextMenu("Draw")]
  public void Draw() {

    BezierCurve curve = GetComponent<BezierCurve>();
    LineRenderer renderer = GetComponent<LineRenderer>();

    List<Vector3> vect = new List<Vector3>();
    Vector3 target = Vector3.zero;
    float point = 0;

    while (point < 1) {

      target = curve.GetPointAt(point);
      vect.Add(target);
      point += 0.03f;
    }
    point = 1f;
    target = curve.GetPointAt(point);
    vect.Add(target);

    renderer.positionCount = vect.Count;
    renderer.SetPositions(vect.ToArray());
  }
}
