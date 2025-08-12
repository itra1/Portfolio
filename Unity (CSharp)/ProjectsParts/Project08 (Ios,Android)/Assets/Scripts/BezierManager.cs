using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierManager : MonoBehaviour {

	public BezierCurve bazierCurve;
	public int resolution;

	private BezierPoint _activeBezier;
	public LineRenderer lineRenderer;

	private bool _isDrawn;

	public void StartDrag(Vector3 point) {
		_isDrawn = true;
		_activeBezier = bazierCurve.AddPointAt(point);
	}

	public void ChangePosition(Vector3 actualPosition) {
		if (!_isDrawn) return;
		if (_activeBezier == null) return;
		_activeBezier.transform.position = actualPosition;
		CorrectAngle();
		DrawnLine();
	}

	public void ClearPoint() {
		_isDrawn = false;
		_activeBezier = null;
		bazierCurve.RemoveAll();
		lineRenderer.positionCount = 0;
	}

	public void AddPoint(Vector3 point) {
		if (!_isDrawn) return;
		
		_activeBezier.transform.position = point;
		CorrectAngle();
		_activeBezier = bazierCurve.AddPointAt(point);
	}

	void CorrectAngle() {
		if (bazierCurve.pointCount <= 2) return;

		BezierPoint before0 = bazierCurve[bazierCurve.pointCount - 1];
		BezierPoint before1 = bazierCurve[bazierCurve.pointCount - 2];
		BezierPoint before2 = bazierCurve[bazierCurve.pointCount - 3];

		Vector3 delta = (before0.position - before2.position).normalized/2;
		before1.handle2 = delta;
		before1.handle1 = -delta;

	}

	public void TrimList(int nummerMax) {
		
		int deleteCount = bazierCurve.pointCount - nummerMax;
		
		for (int i = 0; i < deleteCount; bazierCurve.TrimLast(), i++) ;

		_activeBezier = bazierCurve[bazierCurve.pointCount - 1];
		_activeBezier.handle2 = Vector3.zero;
		_activeBezier.handle1 = Vector3.zero;

	}

	List<Vector3> pointList = new List<Vector3>();
	void DrawnLine() {

		if (bazierCurve.pointCount < 2) return;

		pointList.Clear();

		float startPos = 0;
		float step = 1f / resolution;

		while (startPos < 1) {
			pointList.Add(bazierCurve.GetPointAt(startPos));
				
			startPos += step;
			if (startPos > 0.99) startPos = 1;
		}
		pointList.Add(bazierCurve.GetPointAt(1));

		lineRenderer.positionCount = pointList.Count;
		lineRenderer.SetPositions(pointList.ToArray());
	}


}
