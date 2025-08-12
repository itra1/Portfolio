using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace it.Diagrams
{
	public class BelevedProgressbar : MonoBehaviour
	{
		[SerializeField] private MeshUIRendererHelper _prefab;
		[SerializeField] private float _height;
		[SerializeField] private float beveled = 10;
		[SerializeField] private float _width = 40;

		private void OnDrawGizmosSelected()
		{
			Draw();
		}

		private void Draw(){

			RectTransform itemRect = GetComponent<RectTransform>();

			Mesh mesh = new Mesh { name = "Procedural Mesh" };

			List<Vector3> vertices = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uv = new List<Vector2>();
			List<int> uvInd = new List<int>();

			Vector3 p = Vector3.zero;
			int pi = -1;

			p = itemRect.TransformPoint(new Vector3(itemRect.anchoredPosition.x , itemRect.anchoredPosition.y + _height/2, 0));
			vertices.Add(itemRect.InverseTransformPoint(p)); 
			//vertices.Add(p);
			normals.Add(Vector3.back);
			uv.Add(new Vector2(0, 1));
			Gizmos.DrawWireSphere(p, 0.1f);
			pi++;

			p = itemRect.TransformPoint(new Vector3(itemRect.anchoredPosition.x, itemRect.anchoredPosition.y - _height / 2, 0));
			vertices.Add(itemRect.InverseTransformPoint(p));
			//vertices.Add(p);
			normals.Add(Vector3.back);
			uv.Add(new Vector2(0, 0));
			Gizmos.DrawWireSphere(p, 0.1f);
			pi++;

			p = itemRect.TransformPoint(new Vector3(itemRect.anchoredPosition.x + _width+ beveled, itemRect.anchoredPosition.y + _height / 2, 0));
			vertices.Add(itemRect.InverseTransformPoint(p));
			//vertices.Add(p);
			normals.Add(Vector3.back);
			uv.Add(new Vector2(1, 1));
			Gizmos.DrawWireSphere(p, 0.1f);
			pi++;


			uvInd.Add(pi);
			uvInd.Add(0);
			uvInd.Add(pi - 1);

			p = itemRect.TransformPoint(new Vector3(itemRect.anchoredPosition.x + _width, itemRect.anchoredPosition.y - _height / 2, 0));
			vertices.Add(itemRect.InverseTransformPoint(p));
			//vertices.Add(p);
			normals.Add(Vector3.back);
			uv.Add(new Vector2(1, 0));
			Gizmos.DrawWireSphere(p, 0.1f);
			pi++;


			uvInd.Add(pi);
			uvInd.Add(1);
			uvInd.Add(pi - 1);



			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.uv = uv.ToArray();
			mesh.triangles = uvInd.ToArray();
			_prefab.SetMesh(mesh);
		}

	}
}