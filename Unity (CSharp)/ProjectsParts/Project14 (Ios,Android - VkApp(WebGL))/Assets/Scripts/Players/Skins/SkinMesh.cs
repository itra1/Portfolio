using UnityEngine;

namespace Game.Players.Skins {
	public class SkinMesh : MonoBehaviour {
		[SerializeField] private Material _baseMaterial;
		[SerializeField] private string _colorMaterialProperty = "_Color";
		private Color _color;

		public void Initiate(Color color) {
			_color = color;
			_baseMaterial.SetColor(_colorMaterialProperty, color);

			AfterInitiate();
		}

		protected virtual void AfterInitiate() {

		}

	}
}
