using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace it.Diagrams
{
[RequireComponent(typeof(CanvasRenderer))]
	public class MeshUIRendererHelper : MaskableGraphic
	{
		[FormerlySerializedAs("m_Frame")]
		[SerializeField]
		private Sprite m_Sprite;

		[SerializeField] private Mesh _mesh;

		private CanvasRenderer cr;

		public void SetMesh(Mesh mesh)
		{
			_mesh = mesh;

			List<Color> colors = new List<Color>();

			for(int i = 0; i < _mesh.vertexCount;i++){
				colors.Add(color);
			}
			_mesh.colors = colors.ToArray();

			if (cr == null)
			cr = GetComponent<CanvasRenderer>();
			cr.materialCount = 1;
			cr.SetMesh(_mesh);
			if(material != null)
				cr.SetMaterial(material,0);
		}

		protected override void UpdateGeometry()
		{
			base.UpdateGeometry();
			if (_mesh != null)
				SetMesh(_mesh);
		}

	}
}