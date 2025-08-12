using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements {
	public class AvatarItemView :MonoBehaviour, IPoolable<IMemoryPool> {

		public UnityAction<string> OnClick;

		[SerializeField] private Button _mainButtons;
		[SerializeField] private RectTransform _light;
		[SerializeField] private RawImage _avatarRaw;

		private Texture2D _texture;
		private IMemoryPool _pool;

		public bool IsSelect { get; private set; }

		public string TextureName => _texture.name;

		[Inject]
		public void Constructor() {

		}

		public void Awake() {
			_mainButtons.onClick.AddListener(() => {
				OnClick?.Invoke(_texture.name);
			});
		}

		public void OnDespawned() {
		}

		public void Despawn() {
			_pool.Despawn(this);
		}

		public void OnSpawned(IMemoryPool pool) {
			_pool = pool;
		}

		public void SetTexture(Texture2D texture) {
			_texture = texture;
			_avatarRaw.texture = _texture;
		}

		public void SetSelect(bool isSelect) {
			IsSelect = isSelect;
			_light.gameObject.SetActive(IsSelect);
		}

		public class Factory :PlaceholderFactory<AvatarItemView> { }
	}
}
