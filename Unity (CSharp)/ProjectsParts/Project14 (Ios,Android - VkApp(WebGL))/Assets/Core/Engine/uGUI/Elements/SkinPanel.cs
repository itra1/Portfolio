using Core.Engine.Components.Shop;
using Core.Engine.Components.Skins;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using Core.Engine.Components.Audio;

namespace Core.Engine.uGUI.Elements
{
	public class SkinPanel : MonoBehaviour
	{
		[SerializeField] private Image _icone;
		[SerializeField] private TMP_Text _titleLabel;
		[SerializeField] private Button _selectButton;
		[SerializeField] private RectTransform _selectedRect;

		private RectTransform _rt;
		protected ISkinProvider _skinProvider;
		protected ISkin _skin;
		protected SignalBus _signalBus;

		public RectTransform RT => _rt ??= GetComponent<RectTransform>();
		public void Set(ISkinProvider provider, ISkin product)
		{
			_skinProvider = provider;
			_skin?.UnSubscribeChange(Confirm);
			_skin = product;
			_skin.SubscribeChange(Confirm);

			Confirm();
		}
		private void OnDisable()
		{
			_skin?.UnSubscribeChange(Confirm);
		}
		protected void Confirm()
		{
			_icone.sprite = _skin.Icone;
			_titleLabel.text = _skin.Title;
			_selectButton.gameObject.SetActive(!_skin.IsSelected);
			_selectedRect.gameObject.SetActive(_skin.IsSelected);
			_selectButton.onClick.RemoveAllListeners();
			_selectButton.onClick.AddListener(Activate);
		}
		public void Activate()
		{
			PlayAudio.PlaySound("click");
			_skinProvider.SetActiveSkin(_skin);
		}
	}
}
