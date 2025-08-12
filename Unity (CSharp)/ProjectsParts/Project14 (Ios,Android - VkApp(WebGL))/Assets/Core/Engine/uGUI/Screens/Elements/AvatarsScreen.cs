using Core.Engine.App.Base.Attributes;
using Core.Engine.Components.Audio;
using Core.Engine.Components.Avatars;
using Core.Engine.Components.User;
using Core.Engine.Signals;
using Core.Engine.Utils;
using Engine.uGUI.Screens;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Core.Engine.uGUI.Screens
{
	[PrefabName(ScreenTypes.Avatar)]
	public class AvatarsScreen :Screen
	{
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private AvatarItem _itemPrefab;
		[SerializeField] private Button _applyButton;

		private AvatarItem _selectedAvatar;
		private IAvatarsProvider _avatarsProvider;
		private IUserProvider _userProvider;
		private PrefabPool<AvatarItem> _itemPool;

		[Inject]
		public void Initiate(IAvatarsProvider avatarsProvider,IUserProvider userProvider)
		{
			_avatarsProvider = avatarsProvider;
			_userProvider = userProvider;

			_itemPool = new(_itemPrefab,_scrollRect.content);
			_itemPrefab.gameObject.SetActive(false);
		}

		private void OnEnable()
		{
			SpawnItems();
			_applyButton.interactable = false;
		}

		private void SpawnItems()
		{
			_itemPool.HideAll();

			System.Collections.Generic.Dictionary<string,Texture2D> avatars = _avatarsProvider.Avatars;
			GridLayoutGroup glg = _scrollRect.content.GetComponent<GridLayoutGroup>();
			glg.spacing = new Vector2(25,25);
			float size = (_scrollRect.content.rect.width - glg.padding.left - glg.padding.right - glg.spacing.x) / 2;
			glg.cellSize = new Vector2(size,size);

			foreach (System.Collections.Generic.KeyValuePair<string,Texture2D> item in avatars)
			{
				AvatarItem elem = _itemPool.GetItem();
				elem.Set(item);
				//elem.RT.anchoredPosition = new(elem.RT.anchoredPosition.x, -(i * (elem.RT.rect.height + 5)));
				elem.gameObject.SetActive(true);
				elem.OnSelect = () =>
				{
					if (_selectedAvatar != null)
						_selectedAvatar.SetSelect(false);
					_selectedAvatar = elem;
					_applyButton.interactable = true;
					//elem.SetSelect(true);
				};
			}
			_scrollRect.content.sizeDelta = new Vector2(_scrollRect.content.sizeDelta.x,Mathf.Ceil(avatars.Count / 2f) * (glg.cellSize.y + 25));
		}

		public void BackButtonTouch()
		{
			PlayAudio.PlaySound("click");
			_signalBus.Fire(new UGUIButtonClickSignal() { Name = ButtonTypes.FirstMenuOpen });
		}

		public void ApplyButton()
		{

			_userProvider.SetAvatarName(_selectedAvatar.Avatar.Key);
			Hide();
			// Применить
		}
	}
}
