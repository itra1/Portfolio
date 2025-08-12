using UnityEngine;

namespace Game.Game.Components {
	public class BlackRoundMask :MonoBehaviour, IBlackRoundMask {
		[SerializeField] private SpriteRenderer _blackRoundMask;
		[SerializeField] private Color _blackBack;

		private Material _material;

		public SpriteRenderer Mask => _blackRoundMask;
		public Color BlackColor => _blackBack;
		public Material Material => _material;

		public void Awake() {
			FirstInit();
		}

		[ContextMenu("FirstInit")]
		private void FirstInit() {
			_blackRoundMask.SpriteScaleToOrtoScreenVisible();
			_material = _blackRoundMask.materials[0];
			SetColor(new(0, 0, 0, 0));
			_blackRoundMask.enabled = false;
		}

		private void SetColor(Color targetColor) {
			_material.SetColor("_Color", targetColor);
		}
	}
}
