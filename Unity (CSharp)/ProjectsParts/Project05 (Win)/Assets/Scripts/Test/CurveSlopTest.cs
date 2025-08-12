using UnityEngine;
using FluffyUnderware.Curvy;

[ExecuteInEditMode]
public class CurveSlopTest : MonoBehaviour
{
  public CurvySpline Spline;
  private Vector3 targetPosition;
  Vector3 tangent;
  private void Update()
  {
	 float nearestTF = Spline.GetNearestPointTF(transform.position, Space.World);
	 targetPosition = Spline.Interpolate(nearestTF, Space.World);

    Vector3 pos;
    Vector3 orientation;
    GetInterpolatedSourcePosition(nearestTF, out pos, out tangent, out orientation);


  }

  private void OnDrawGizmosSelected()
  {

	 Gizmos.DrawLine(transform.position, targetPosition);
    Gizmos.color = Color.green;

    Gizmos.DrawLine(targetPosition, targetPosition + tangent*200);

  }

  protected void GetInterpolatedSourcePosition(float tf, out Vector3 interpolatedPosition, out Vector3 tangent, out Vector3 up)
  {
    CurvySpline spline = Spline;
    Transform splineTransform = spline.transform;

    float localF;
    CurvySplineSegment currentSegment = spline.TFToSegment(tf, out localF);
    if (ReferenceEquals(currentSegment, null) == false)
    {
      currentSegment.InterpolateAndGetTangent(localF, out interpolatedPosition, out tangent);
      up = currentSegment.GetOrientationUpFast(localF);
    }

    else
    {
      interpolatedPosition = Vector3.zero;
      tangent = Vector3.zero;
      up = Vector3.zero;
    }

    interpolatedPosition = splineTransform.TransformPoint(interpolatedPosition);
    tangent = splineTransform.TransformDirection(tangent);
    up = splineTransform.TransformDirection(up);
  }


}
