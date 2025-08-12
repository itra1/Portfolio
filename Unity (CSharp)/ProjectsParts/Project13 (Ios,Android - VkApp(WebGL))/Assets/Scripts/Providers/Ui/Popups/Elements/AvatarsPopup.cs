using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Common.Attributes;
using Game.Providers.Avatars;
using Game.Providers.Profile;
using Game.Providers.Ui.Elements;
using Game.Providers.Ui.Popups.Base;
using Game.Providers.Ui.Popups.Common;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Popups.Elements
{
	[PrefabName(PopupsNames.Avatars)]
	public class AvatarsPopup : Popup
	{
		[SerializeField] private ScrollRect _scrollRect;
		[SerializeField] private Button _backButton;

		private IAvatarsProvider _avatarProvider;
		private IProfileProvider _profileProvider;
		private AvatarItemView.Factory _factory;
		private List<AvatarItemView> _list = new();
		private string _textureName;

		[Inject]
		public void Constructor(IProfileProvider profileProvider, IAvatarsProvider avatarProvider, AvatarItemView.Factory factory)
		{
			_profileProvider = profileProvider;
			_avatarProvider = avatarProvider;
			_factory = factory;
		}

		protected override void Awake()
		{
			base.Awake();
			MakeList();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetSelect(_profileProvider.Avatar);
		}

		private void MakeList()
		{
			if (_list.Count > 0)
				return;
			var index = -1;
			foreach (var element in _avatarProvider.Avatars)
			{
				index++;
				var inst = _factory.Create();
				inst.SetTexture(element.Value);
				inst.OnClick = SetSelect;
				var rectTransform = inst.GetComponent<RectTransform>();
				rectTransform.SetParent(_scrollRect.content);
				rectTransform.localScale = Vector3.one;
				_list.Add(inst);
			}
			var glg = _scrollRect.content.GetComponent<GridLayoutGroup>();
			var rows = Mathf.CeilToInt(index / 3);
			_scrollRect.content.sizeDelta = new(_scrollRect.content.sizeDelta.x, (rows * glg.cellSize.y) + (glg.spacing.y * (rows - 1)));
		}

		public void SetSelect(string name)
		{
			_textureName = name;
			for (var i = 0; i < _list.Count; i++)
				_list[i].SetSelect(_list[i].TextureName == name);
		}

		public void CloseButtonTouch()
		{
			Hide().Forget();
		}

		public void SelectButtonTouch()
		{
			_profileProvider.SetAvatar(_textureName);
			Hide().Forget();
		}
	}
}
