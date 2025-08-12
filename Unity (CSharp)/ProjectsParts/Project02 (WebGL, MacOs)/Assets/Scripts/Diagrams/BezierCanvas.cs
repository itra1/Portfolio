using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace it.Diagrams
{
	public class BezierCanvas : MonoBehaviour
	{
		[SerializeField] private RectTransform _curveParent;
		[SerializeField] private bool _localSpace;
		[SerializeField] private float _radius = 1f;
		private List<CurvePoint> _points = new List<CurvePoint>();
		[Range(0.001f, 1f)]
		public float _step = 0.1f;

		public List<CurvePoint> Points { get => _points; set => _points = value; }

		//private void OnDrawGizmosSelected()
		//{
		//	if (_points.Count == 1) return;

		//	var lst = GetAllPoints();

		//	//for (int i = 0; i < lst.Count; i++)
		//	//{
		//	//	Gizmos.DrawWireSphere(lst[i] + (_localSpace ? (Vector2)_curveParent.position : Vector2.zero), _radius);
		//	//}
		//}

		public List<Vector2> GetAllPoints()
		{
			List<Vector2> _pointsResult = new List<Vector2>();

			float t = 0;

			_pointsResult.Add(GetPoint(t, _points[0], _points[1]));

			while (t < 0.99999f)
			{
				t += _step;
				t = Mathf.Clamp(t, 0, 0.99999f);
				AddPointByTime(t, ref _pointsResult);
				if (t >= 0.99999f) break;

			}
			AddPointByTime(1, ref _pointsResult);

			return _pointsResult;

		}

		private void AddPointByTime(float t, ref List<Vector2> p)
		{
			float allSteps = (_points.Count - 1);
			float timeOneStep = 1f / allSteps;

			int step = (int)(t / timeOneStep);

			float timeInStep = (t % timeOneStep) / timeOneStep;

			if (t == 1)
			{
				timeInStep = 1;
				step = _points.Count - 2;
			}

			p.Add(GetPoint(timeInStep, _points[step], _points[step + 1]));

		}

		public Vector2 GetPoint(float t, CurvePoint p1, CurvePoint p2)
		{
			return (p1.Center * ((1 - t) * (1 - t) * (1 - t)))
					+ (3 * p1.NextPos * t * ((1 - t) * (1 - t)))
					+ (3 * p2.PrevPos * (t * t) * (1 - t))
					+ (p2.Center * (t * t * t));
		}

		[System.Serializable]
		public class CurvePoint
		{
			public Vector2 Prev;
			public Vector2 Center;
			public Vector2 Next;
			public Vector2 PrevPos => Center + Prev;
			public Vector2 NextPos => Center + Next;
		}

	}
}