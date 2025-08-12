using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace it.Diagrams
{
	public class RingDiagram : MonoBehaviour
	{
		[SerializeField] private Color _color = Color.white;
		[SerializeField] private MeshUIRendererHelper _renderer;
		[SerializeField] private TextMeshProUGUI _valueLabel;
		[SerializeField] private float _radius = 20;
		[SerializeField] private float _startAngle = 0;
		[SerializeField] private float _maxAngle = 0;
		[Range(0.001f, 90)]
		[SerializeField] private float _degreeStep = 1;
		[Range(0.001f, 90)]
		[SerializeField] private float _thickness = 10;
		[SerializeField] private int _endRoundCorner = 0;

		private RectTransform _rendererRect;
		private float value = 1;

		private void Awake()
		{
			SetValue(0);
		}

		[ContextMenu("Reset")]
		public void ResetData()
		{
			SetValue(0f);
		}

		public void SetValue(float val)
		{
			this.value = val;
			Draw();
		}

		private void Draw()
		{
			float fullLenght = _maxAngle - _startAngle;

			_valueLabel.text = string.Format("{0}%", Mathf.Round(value*100));

			if (_rendererRect == null)
				_rendererRect = _renderer.GetComponent<RectTransform>();

			Vector2 vector = Vector2.up * _radius;
			vector = Quaternion.Euler(0, 0, -_startAngle) * vector;
			float currentAngle = -_startAngle;

			Vector2 point = Vector2.zero + vector;

			List<SpawnPoints> pointeList = new List<SpawnPoints>();

			do
			{
				point = Vector2.zero + vector;

				pointeList.Add(new SpawnPoints()
				{
					Center = point,
					Out = point + (vector.normalized * (_thickness / 2)),
					Inner = point - (vector.normalized * (_thickness / 2)),
					Lenght = (currentAngle - _startAngle) / fullLenght
				});

				vector = Quaternion.Euler(0, 0, -_degreeStep) * vector;
				currentAngle -= _degreeStep;
			} while (currentAngle > -(_startAngle + (fullLenght * value)));

			vector = Vector2.up * _radius;
			vector = Quaternion.Euler(0, 0, -(_startAngle + (fullLenght * value))) * vector;
			point = Vector2.zero + vector;

			pointeList.Add(new SpawnPoints()
			{
				Center = point,
				Out = point + (vector.normalized * (_thickness / 2)),
				Inner = point - (vector.normalized * (_thickness / 2)),
				Lenght = 1
			});

			DrawMesh(pointeList);
		}

		public class SpawnPoints
		{
			public Vector2 Out;
			public Vector2 Center;
			public Vector2 Inner;
			public float Lenght;
		}

		private void DrawMesh(List<SpawnPoints> pointeList)
		{
			_renderer.color = _color;

			Mesh mesh = new Mesh { name = "Procedural Mesh" };

			List<Vector3> vertices = new List<Vector3>(); // new Vector3[points.Count * 2];
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uv = new List<Vector2>();
			List<int> uvInd = new List<int>();

			if (value == 0)
			{
				mesh.vertices = vertices.ToArray();
				mesh.normals = normals.ToArray();
				mesh.uv = uv.ToArray();
				mesh.triangles = uvInd.ToArray();
				_renderer.SetMesh(mesh);
				return;
			}

			int pi = -1;
			Vector3 p = Vector3.zero;
			foreach (var point in pointeList)
			{
				p = _rendererRect.TransformPoint(point.Out);
				vertices.Add(_rendererRect.InverseTransformPoint(p));
				normals.Add(Vector3.back);
				uv.Add(new Vector2(point.Lenght, 1));
				pi++;

				p = _rendererRect.TransformPoint(point.Center);
				vertices.Add(_rendererRect.InverseTransformPoint(p));
				normals.Add(Vector3.back);
				uv.Add(new Vector2(point.Lenght, 0.5f));
				pi++;

				p = _rendererRect.TransformPoint(point.Inner);
				vertices.Add(_rendererRect.InverseTransformPoint(p));
				normals.Add(Vector3.back);
				uv.Add(new Vector2(point.Lenght, 0));
				pi++;

				if (pi >= 3)
				{

					uvInd.Add(pi - 4);
					uvInd.Add(pi - 5);
					uvInd.Add(pi - 2);

					uvInd.Add(pi - 4);
					uvInd.Add(pi - 2);
					uvInd.Add(pi - 1);

					uvInd.Add(pi);
					uvInd.Add(pi - 3);
					uvInd.Add(pi - 1);

					uvInd.Add(pi - 3);
					uvInd.Add(pi - 4);
					uvInd.Add(pi - 1);
				}

			}

			int endPi = pi;

			if (_endRoundCorner > 0)
			{

				// Рисуем концевикиж

				// начало
				float degreeStep = 180f / (_endRoundCorner + 1);
				Gizmos.color = Color.green;
				SpawnPoints startPoint = pointeList[0];
				Vector2 sVector = startPoint.Out - startPoint.Center;
				float sDegree = 0;

				for (int i = 0; i < _endRoundCorner; i++)
				{

					sDegree += degreeStep;
					sVector = Quaternion.Euler(0, 0, degreeStep) * sVector;

					Vector2 sPoint = startPoint.Center + sVector;
					p = _rendererRect.TransformPoint(sPoint);

					vertices.Add(_rendererRect.InverseTransformPoint(p));
					normals.Add(Vector3.back);
					uv.Add(new Vector2(0, 1));
					pi++;

					if (i == 0)
					{

						uvInd.Add(pi);
						uvInd.Add(0);
						uvInd.Add(1);
						continue;
					}

					uvInd.Add(pi - 1);
					uvInd.Add(1);
					uvInd.Add(pi);

					if (i == _endRoundCorner - 1)
					{
						uvInd.Add(1);
						uvInd.Add(pi);
						uvInd.Add(2);
						continue;
					}

				}
				startPoint = pointeList[pointeList.Count - 1];
				sVector = startPoint.Out - startPoint.Center;
				sDegree = 0;

				for (int i = 0; i < _endRoundCorner; i++)
				{

					sDegree -= degreeStep;
					sVector = Quaternion.Euler(0, 0, -degreeStep) * sVector;

					Vector2 sPoint = startPoint.Center + sVector;
					p = _rendererRect.TransformPoint(sPoint);

					vertices.Add(_rendererRect.InverseTransformPoint(p));
					normals.Add(Vector3.back);
					uv.Add(new Vector2(1, 1));
					pi++;

					if (i == 0)
					{
						uvInd.Add(pi);
						uvInd.Add(endPi - 2);
						uvInd.Add(endPi - 1);
						continue;
					}
					uvInd.Add(pi);
					uvInd.Add(endPi - 1);
					uvInd.Add(pi - 1);

					if (i == _endRoundCorner - 1)
					{
						uvInd.Add(endPi);
						uvInd.Add(pi);
						uvInd.Add(endPi - 1);
						continue;
					}

				}

			}

			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = uv.ToArray();
			mesh.triangles = uvInd.ToArray();
			_renderer.SetMesh(mesh);

		}

	}
}