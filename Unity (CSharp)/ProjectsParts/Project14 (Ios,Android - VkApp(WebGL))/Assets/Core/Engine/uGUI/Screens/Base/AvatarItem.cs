
using Core.Engine.Components.Shop;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Engine.uGUI.Screens
{
	public class AvatarItem : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnSelect;

		[SerializeField] private RawImage _icone;
		[SerializeField] private GameObject _selectedBack;

		protected IShopProduct _product;

		private KeyValuePair<string, Texture2D> _avatar;
		private RectTransform _rt;
		public RectTransform RT => _rt = _rt != null ? _rt : GetComponent<RectTransform>();
		public KeyValuePair<string, Texture2D> Avatar { get => _avatar; set => _avatar = value; }

		public void Set(KeyValuePair<string, Texture2D> avatar)
		{
			Avatar = avatar;

			Confirm();
			SetSelect(false);
		}

		private void Confirm()
		{
			_icone.texture = Avatar.Value;
		}

		public void SetSelect(bool isSelect)
		{
			if (isSelect)
				OnSelect?.Invoke();

			_selectedBack.gameObject.SetActive(isSelect);
		}

		public void SelectButton()
		{
			SetSelect(true);
		}

	}
}
