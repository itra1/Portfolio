using Game.Providers.Profile;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements
{
	public class AvatarSelectView : MonoBehaviour, IPoolable<Texture2D, IMemoryPool>, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		[SerializeField] private RawImage _avatar;
		[SerializeField] private RectTransform _hoverImage;

		private IMemoryPool _pool;
		private Texture2D _texture;
		private IProfileProvider _profileProvider;
		public string TextureName => _texture.name;

		[Inject]
		public void Constructor(IProfileProvider profileProvider)
		{
			_profileProvider = profileProvider;
		}

		public void OnEnable()
		{
			SetHover(false);
		}

		private void SetHover(bool isHover)
		{
			_hoverImage.gameObject.SetActive(isHover);
		}

		public void OnDespawned() { }

		public void Despawned()
		{
			_pool.Despawn(this);
		}

		public void OnSpawned(Texture2D texture, IMemoryPool pool)
		{
			_pool = pool;
			_texture = texture;
			_avatar.texture = _texture;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			SetHover(true);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			SetHover(false);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			_profileProvider.SetAvatar(_texture.name);
		}

		public class Factory : PlaceholderFactory<Texture2D, AvatarSelectView> { }
	}
}
