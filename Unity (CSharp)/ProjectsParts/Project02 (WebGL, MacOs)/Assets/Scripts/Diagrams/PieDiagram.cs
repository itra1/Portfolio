using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Diagrams
{
	public class PieDiagram : MonoBehaviour
	{
		[SerializeField] private MeshUIRendererHelper _prefab;
		[SerializeField] private float _radius = 20;
		[Range(0.001f, 90)]
		[SerializeField] private float _degreeStep = 1;

		private PoolList<MeshUIRendererHelper> _poopItem;
		private List<PieItem> _pies = new List<PieItem>();
		private float _summary;

		public class PieItem
		{
			public Color Color = Color.white;
			public float Value;
		}

		public void SetData(List<PieItem> items)
		{
			_pies = items;
			Draw();
		}

		private void Draw()
		{
			if (_poopItem == null)
				_poopItem = new PoolList<MeshUIRendererHelper>(_prefab.gameObject, transform);

			_poopItem.HideAll();

			Vector2 vector = Vector2.right * _radius;
			float degree = -90;

			_summary = 0;
			for (int i = 0; i < _pies.Count; i++)
			{
				_summary += _pies[i].Value;
			}
			float itemSize = 360 / _summary;


			for (int i = 0; i < _pies.Count; i++)
			{
				List<Vector2> pointList = new List<Vector2>();
				Gizmos.color = _pies[i].Color;

				Vector2 startVector = vector;
				float startDegree = degree;

				float targetDegree = degree - (itemSize * _pies[i].Value);

				Vector2 point = Vector2.zero + vector;

				do
				{
					point = Vector2.zero + vector;
					pointList.Add(point);
					vector = Quaternion.Euler(0, 0, -_degreeStep) * vector;
					degree -= _degreeStep;

				} while (degree > targetDegree);

				vector = startVector;
				vector = Quaternion.Euler(0, 0, -itemSize * _pies[i].Value) * vector;
				point = Vector2.zero + vector;
				pointList.Add(point);
				degree = startDegree -= itemSize * _pies[i].Value;

				DrawMesh(_pies[i].Color, pointList);
			}

		}
		private void DrawMesh(Color color, List<Vector2> pointeList)
		{
			var item = _poopItem.GetItem();
			item.color = color;
			RectTransform itemRect = item.GetComponent<RectTransform>();

			Mesh mesh = new Mesh { name = "Procedural Mesh" };

			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uv = new List<Vector2>();
			List<int> uvInd = new List<int>();

			Vector3 p = Vector3.zero;
			int pi = -1;

			p = itemRect.TransformPoint(p);
			vertices.Add(itemRect.InverseTransformPoint(p));
			normals.Add(Vector3.back);
			uv.Add(new Vector2(0, 0));
			pi++;

			for (int i = 0; i < pointeList.Count; i++)
			{

				p = itemRect.TransformPoint(pointeList[i]);
				vertices.Add(itemRect.InverseTransformPoint(p));
				normals.Add(Vector3.back);
				uv.Add(new Vector2(1, 1));
				pi++;

				if (pi > 1)
				{
					uvInd.Add(pi);
					uvInd.Add(0);
					uvInd.Add(pi - 1);
				}
			}

			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = uv.ToArray();
			mesh.triangles = uvInd.ToArray();
			item.SetMesh(mesh);
		}


	}
}