using Core.Engine.Components.Game;
using Core.Engine.Components.Skins;
using UnityEngine;
using Zenject;

namespace Scripts.Players.Skins {
	public class PlayerMeshItemSkin : ItemMesh {
		[SerializeField] private Color _blobColor;
		[SerializeField] private GameObject _prefab;
		//private PlayerSkinController _skinController;
		private ISceneComponent _sceneComponentsBase;

		public Color BlobColor { get => _blobColor; set => _blobColor = value; }

		[Inject]
		public void Initialize(DiContainer diController, ISceneComponent sceneComponentsBase) {
			_sceneComponentsBase = sceneComponentsBase;

		}

		public override bool Confirm() {
			if (!(_sceneComponentsBase as Component).TryGetComponent<IPlayerObject>(out var playerParent))
				return false;

			if (playerParent.PlayerGameObject.TryGetComponent<PlayerSkinController>(out var skinController)) {
				skinController.SetSkin(_blobColor, _prefab);
			} else {
				return false;
			}

			return true;
		}
	}
}
