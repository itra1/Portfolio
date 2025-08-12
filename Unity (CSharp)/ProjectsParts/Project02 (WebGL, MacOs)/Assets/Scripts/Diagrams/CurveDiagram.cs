using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using static it.Diagrams.RingDiagram;

namespace it.Diagrams
{
	public class CurveDiagram : MonoBehaviour
	{
		[SerializeField] private MeshUIRendererHelper _meshRenderer;
		[SerializeField] private MeshUIRendererHelper _lineRenderer;
		[SerializeField] private TextMeshProUGUI _datePrefab;
		[SerializeField] private CurveDiagramPoint _pointPrefab;
		[SerializeField] private RectTransform _diagramRect;
		[SerializeField] private float _lineThickness = 1f;
		[SerializeField] private float _heightItem = 1;

		private PoolList<TextMeshProUGUI> _datePool;
		private PoolList<CurveDiagramPoint> _pointPool;
		private List<DiagramItem> _data;
		private BezierCanvas _bezier;
		private Vector2[] _positions;
		private float _minValue;
		private float _maxValue;
		private float _valuesPeriod;
		private float _horizontalMinut;
		private float _maxDiagramValue;
		private float _verticalGrad;
		private int _heightCount;
		private System.DateTime _minDate;
		private System.DateTime _maxDate;
		private int _horizontalCountSeparate;
		private DateTime _startDateVisible;
		private DateTime _endDateVisible;
		private PokerStatisticDateInticator _dateDelimetr;
		private DiagramDateVisible _vo;

		public class DiagramItem
		{
			public System.DateTime Date;
			public float Value;
		}

		public void SetData(List<DiagramItem> data, System.DateTime startDate, System.DateTime endDate, PokerStatisticDateInticator dateDelimetr, DiagramDateVisible visibleOptions, int horiszontalCountSeparate)
		{
			_horizontalCountSeparate = horiszontalCountSeparate + 1;
			_startDateVisible = startDate;
			_endDateVisible = endDate;
			_dateDelimetr = dateDelimetr;
			_data = data;
			_vo = visibleOptions;
			_datePrefab.gameObject.SetActive(false);
			if (_datePool == null)
				_datePool = new PoolList<TextMeshProUGUI>(_datePrefab.gameObject, _datePrefab.transform.parent);

			_pointPrefab.gameObject.SetActive(false);
			if (_pointPool == null)
				_pointPool = new PoolList<CurveDiagramPoint>(_pointPrefab.gameObject, _pointPrefab.transform.parent);

			Draw();
		}

		//private void OnDrawGizmosSelected()
		//{
		//	_data = new List<DiagramItem>();
		//	_data.Add(new DiagramItem() { Date = System.DateTime.Now.AddDays(-1), Value = 2 });
		//	_data.Add(new DiagramItem() { Date = System.DateTime.Now, Value = 1 });
		//	_data.Add(new DiagramItem() { Date = System.DateTime.Now.AddDays(1), Value = 3 });
		//	_data.Add(new DiagramItem() { Date = System.DateTime.Now.AddDays(2), Value = 0.5f });

		//	_datePrefab.gameObject.SetActive(false);
		//	if (_datePool == null)
		//		_datePool = new PoolList<TextMeshProUGUI>(_datePrefab.gameObject, _datePrefab.transform.parent);

		//	_pointPrefab.gameObject.SetActive(false);
		//	if (_pointPool == null)
		//		_pointPool = new PoolList<CurveDiagramPoint>(_pointPrefab.gameObject, _pointPrefab.transform.parent);

		//	Draw();
		//}

		private void Draw()
		{
			//FindDates();

			_minDate = _startDateVisible;
			_maxDate = _endDateVisible;


			var deltaTime = _endDateVisible - _startDateVisible;
			var delimetr = deltaTime / _horizontalCountSeparate;

			var oldData = new List<DiagramItem>(_data);
			_data.Clear();

			for (int i = 0; i < _horizontalCountSeparate - 1; i++)
			{
				var startDate = _startDateVisible.AddMilliseconds((delimetr * i).TotalMilliseconds);
				var endDate = _startDateVisible.AddMilliseconds((delimetr * (i + 1)).TotalMilliseconds);
				float value = 0;
				for (int y = 0; y < oldData.Count; y++)
				{
					if (oldData[y].Date > startDate && oldData[y].Date <= endDate)
						value += oldData[y].Value;
				}
				_data.Add(new DiagramItem() { Date = endDate, Value = value });
			}


			FindMinMax();
			CalcHorizontal();
			CalcVerticalValues();
			FillPoints();
			SpawmDates();
			SpawmPoint();

			DrawBorder();

		}

		private void SpawmPoint()
		{

			_pointPool.HideAll();

			RectTransform pRt = _pointPrefab.transform.parent.GetComponent<RectTransform>();

			for (int i = 1; i < _positions.Length - 1; i++)
			{
				var itm = _pointPool.GetItem();
				itm.gameObject.SetActive(true);
				RectTransform rt = itm.GetComponent<RectTransform>();
				rt.anchoredPosition = new Vector2(_meshRenderer.rectTransform.rect.width * _positions[i].x, _meshRenderer.rectTransform.rect.height * _positions[i].y);
				itm.SetData(new Vector2(_positions[i].x * pRt.rect.width, _positions[i].y * pRt.rect.height), _data[i-1].Value);
				itm.SelectUnset();
			}

		}

		private void SpawmDates()
		{
			RectTransform pRect = _datePrefab.GetComponent<RectTransform>();
			_datePool.HideAll();

			for (int i = 1; i < _positions.Length - 1; i++)
			{
				var itm = _datePool.GetItem();
				itm.gameObject.SetActive(true);
				RectTransform rt = itm.GetComponent<RectTransform>();
				rt.anchoredPosition = new Vector2(_meshRenderer.rectTransform.rect.width * _positions[i].x, pRect.anchoredPosition.y);

				if ((_vo & DiagramDateVisible.ChetClear) != 0 && i % 2 == 0)
					rt.gameObject.SetActive(false);

				if ((_vo & DiagramDateVisible.ChetOffset) != 0 && i % 2 == 0)
					rt.anchoredPosition = new Vector2(_meshRenderer.rectTransform.rect.width * _positions[i].x, pRect.anchoredPosition.y-10);

				switch (_dateDelimetr)
				{
					case PokerStatisticDateInticator.Minute:
						itm.text = _data[i - 1].Date.ToString("mm");
						break;
					case PokerStatisticDateInticator.Time:
						itm.text = _data[i - 1].Date.ToString("t");
						break;
					case PokerStatisticDateInticator.Date:
						itm.text = _data[i - 1].Date.ToString("dd.MM");
						break;
					case PokerStatisticDateInticator.DateTime:
						itm.text = _data[i - 1].Date.ToString("dd.MM") + "\n" + _data[i - 1].Date.ToString("HH:dd");
						break;
					case PokerStatisticDateInticator.Month:
						itm.text = _data[i - 1].Date.ToString("MMM");
						break;
				}

			}

		}

		private void CalcHorizontal()
		{
			System.TimeSpan ts = _maxDate - _minDate;
			_horizontalMinut = _diagramRect.rect.width / (float)ts.TotalMinutes;
			//_horizontalMinut = 100f / 60f;
		}
		private void FindMinMax()
		{
			_minValue = float.MaxValue;
			_maxValue = float.MinValue;
			for (int i = 0; i < _data.Count; i++)
			{
				if (_data[i].Value < _minValue) _minValue = _data[i].Value;
				if (_data[i].Value > _maxValue) _maxValue = _data[i].Value;
			}

			_valuesPeriod = _maxValue - _minValue;

		}
		private void FindDates()
		{
			_minDate = System.DateTime.MaxValue;
			_maxDate = System.DateTime.MinValue;
			for (int i = 0; i < _data.Count; i++)
			{
				if (_data[i].Date < _minDate) _minDate = _data[i].Date;
				if (_data[i].Date > _maxDate) _maxDate = _data[i].Date;
			}
		}
		private void CalcVerticalValues()
		{
			_maxDiagramValue = _valuesPeriod;
			float visible = _diagramRect.rect.height /** (1 / 1.2f)*/;
			_heightCount = Mathf.Max((int)(visible / 90f), 1);
			_verticalGrad = visible / _heightCount;

			//float itemValue = Mathf.Ceil(_maxDiagramValue / _heightCount / 50),1)*50;
			float itemValue = Mathf.Max(Mathf.Ceil(_maxDiagramValue / _heightCount), 1);
			_maxDiagramValue = itemValue * _heightCount;
			_heightItem = _verticalGrad / (_maxDiagramValue / _heightCount);
		}
		private void FillPoints()
		{
			float itemwWidth = (_diagramRect.rect.width * 0.92f) / _data.Count;

			_positions = new Vector2[_data.Count + 2];

			for (int i = 0; i < _data.Count; i++)
			{
				if (i == 0)
				{
					_positions[0] = new Vector2(0, (float)(((_data[i].Value - _minValue) * _heightItem) / (float)_diagramRect.rect.height));
				}

				_positions[i + 1] = new Vector2(((float)i / ((float)_data.Count - 1) * 0.92f) + 0.04f, (float)(((_data[i].Value - _minValue) * _heightItem) / (float)_diagramRect.rect.height));
				if (i == _data.Count - 1)
				{
					_positions[_data.Count + 1] = new Vector2(1, (float)(((_data[i].Value - _minValue) * _heightItem) / (float)_diagramRect.rect.height));
				}
			}

		}

		private void DrawBorder()
		{
			var min = Vector2.zero;
			var max = new Vector2(_diagramRect.rect.width, _diagramRect.rect.height);

			List<BezierCanvas.CurvePoint> bPoints = new List<BezierCanvas.CurvePoint>();

			for (int i = 0; i < _positions.Length; i++)
			{

				BezierCanvas.CurvePoint bc = new BezierCanvas.CurvePoint();
				bc.Center = _diagramRect.TransformPoint(new Vector2(_positions[i].x * _diagramRect.rect.width, _positions[i].y * _diagramRect.rect.height));
				//Gizmos.DrawWireSphere(bc.Center, 3);
				bPoints.Add(bc);
			}
			for (int i = 0; i < bPoints.Count; i++)
			{
				if (i == 0)
					bPoints[i].Prev = new Vector2(-(bPoints[i + 1].Center.x - bPoints[i].Center.x) / 3, 0);
				else
					bPoints[i].Prev = new Vector2((bPoints[i - 1].Center.x - bPoints[i].Center.x) / 3, 0);

				if (i == bPoints.Count - 1)
					bPoints[i].Next = new Vector2(-(bPoints[i - 1].Center.x - bPoints[i].Center.x) / 3, 0);
				else
					bPoints[i].Next = new Vector2((bPoints[i + 1].Center.x - bPoints[i].Center.x) / 3, 0);
			}

			if (_bezier == null)
				_bezier = GetComponent<BezierCanvas>();

			_bezier.Points = bPoints;
			List<Vector2> allPoints = _bezier.GetAllPoints();
			DrawMesh(min, max, allPoints);
			DrawLine(min, max, allPoints);
		}

		private class CurvePoint
		{
			public Vector3 Up;
			public Vector3 Center;
			public Vector3 Down;
		}


		private void DrawLine(Vector3 min, Vector3 max, List<Vector2> points)
		{
			Mesh mesh = new Mesh
			{
				name = "Procedural Mesh"
			};

			Vector3 size = new Vector3(max.x - min.x, max.y - min.y);

			List<CurvePoint> curvePoints = new List<CurvePoint>();

			for (int i = 0; i < points.Count; i++)
			{
				Vector3 downPoint = _diagramRect.InverseTransformPoint(points[i]);

				Vector3 nextPoint = downPoint + Vector3.right;
				Vector3 prevPoint = downPoint + Vector3.left;

				if (i < points.Count - 1)
					nextPoint = _diagramRect.InverseTransformPoint(points[i + 1]);
				if (i > 0)
					prevPoint = _diagramRect.InverseTransformPoint(points[i - 1]);

				Vector3 nextVector = nextPoint - downPoint;
				Vector3 prevVector = prevPoint - downPoint;

				float anglefull = Vector3.SignedAngle(nextVector, prevVector, Vector3.forward);
				float angle = anglefull / 2;
				angle = Mathf.Abs(angle);
				float thick = _lineThickness / 2;
				float fiag = thick / Mathf.Cos((180 - angle - 90) * (Mathf.PI / 180));

				Vector3 p = Quaternion.Euler(0, 0, -(anglefull < 0 ? (180 - angle) : angle)) * (prevVector.normalized * fiag);

				CurvePoint c = new CurvePoint()
				{
					Center = downPoint
				,
					Up = downPoint + p
				,
					Down = downPoint - p
				};

				curvePoints.Add(c);
			}

			List<Vector3> vertices = new List<Vector3>(); // new Vector3[points.Count * 2];
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uv = new List<Vector2>();
			List<int> uvInd = new List<int>();

			int pi = -1;
			List<Vector3> pp = new List<Vector3>();

			for (int i = 0; i < curvePoints.Count; i++)
			{

				if (i >= curvePoints.Count - 1) continue;

				Vector3 downPoint = curvePoints[i].Up;
				pp.Add(downPoint);
				vertices.Add(downPoint);
				normals.Add(Vector3.back);
				uv.Add(new Vector2((downPoint.x - min.x) / size.x, 1));
				pi++;

				downPoint = curvePoints[i].Down;
				pp.Add(downPoint);
				vertices.Add(downPoint);
				normals.Add(Vector3.back);
				uv.Add(new Vector2((downPoint.x - min.x) / size.x, 0));
				pi++;

				if (pi >= 2)
				{
					uvInd.Add(pi - 3);
					uvInd.Add(pi - 1);
					uvInd.Add(pi - 2);
					uvInd.Add(pi - 2);
					uvInd.Add(pi - 1);
					uvInd.Add(pi);
				}

			}
			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = uv.ToArray();
			mesh.triangles = uvInd.ToArray();

			_lineRenderer.SetMesh(mesh);
			_lineRenderer.transform.SetAsLastSibling();

		}



		public void DrawMesh(Vector3 min, Vector3 max, List<Vector2> points)
		{
			Mesh mesh = new Mesh
			{
				name = "Procedural Mesh"
			};

			Vector3 size = new Vector3(max.x - min.x, max.y - min.y);

			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uv = new List<Vector2>();

			List<int> uvInd = new List<int>();

			int pi = -1;
			List<Vector3> pp = new List<Vector3>();

			for (int i = 0; i < points.Count; i++)
			{
				Vector3 downPoint = _diagramRect.InverseTransformPoint(points[i]);
				pp.Add(downPoint);
				pi++;
				vertices.Add(downPoint);
				normals.Add(Vector3.back);
				uv.Add(new Vector2((downPoint.x - min.x) / size.x, (downPoint.y - min.y) / size.y));

				if (pi >= 2)
				{
					uvInd.Add(pi - 2);
					uvInd.Add(pi - 1);
					uvInd.Add(pi);
				}

				Vector3 oldP = downPoint;

				downPoint = new Vector3(downPoint.x, 0, 0);
				pp.Add(downPoint);
				pi++;
				vertices.Add(downPoint);
				normals.Add(Vector3.back);
				uv.Add(new Vector2((downPoint.x - min.x) / size.x, (downPoint.y - min.y) / size.y));

				if (pi >= 2)
				{
					uvInd.Add(pi);
					uvInd.Add(pi - 1);
					uvInd.Add(pi - 2);
				}

			}
			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = uv.ToArray();
			mesh.triangles = uvInd.ToArray();

			_meshRenderer.SetMesh(mesh);
			_meshRenderer.transform.SetAsLastSibling();

		}


	}
}