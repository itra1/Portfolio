using Core.Elements.Windows.Picture.Data;
using Core.Materials.Attributes;
using Cysharp.Threading.Tasks;
using Elements.FloatingWindow.Presenter.WindowAdapters.Base;
using Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components;
using Elements.Windows.Base.Data.Utils;
using Elements.Windows.Picture.Presenter;
using UnityEngine;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Picture
{
	[MaterialData(typeof(PictureMaterialData))]
	public class PicturePresenterAdapter : WindowPresenterAdapterBase<PicturePresenter>
	{
		[SerializeField] private WindowHeader _header;
		
		protected WindowHeader Header => _header;
		
		public override UniTask<bool> PreloadAsync()
		{
			_header.SetTitle(Material.Name);
			_header.SetIconSprite(Settings.GetWindowMaterialIconSprite(Material.GetIconType()));
			
			_header.Closed += OnHeaderClosed;

			return base.PreloadAsync();
		}

		public override void Unload()
		{
			_header.Closed -= OnHeaderClosed;
			
			base.Unload();
		}
		
		public override void UpdateContent()
		{
			var imageRectTransform = Adaptee.ImageRectTransform;
			var imageSizeDelta = imageRectTransform.sizeDelta;
			var imageSizeDeltaX = imageSizeDelta.x;
			var imageSizeDeltaY = imageSizeDelta.y;
			
			Adaptee.ResizeImage();
			
			imageSizeDelta = imageRectTransform.sizeDelta;
			
			var imageAspectRatioX = imageSizeDeltaX != 0f ? imageSizeDelta.x / imageSizeDeltaX : 0f;
			var imageAspectRatioY = imageSizeDeltaY != 0f ? imageSizeDelta.y / imageSizeDeltaY : 0f;
			
			imageRectTransform.anchoredPosition *= new Vector2(imageAspectRatioY, imageAspectRatioX);
		}
		
		protected override void EnableComponents()
		{
			base.EnableComponents();
			
			if (_header != null && !_header.Visible && IsTriggerOn)
				_header.Show(true);
		}
        
		protected override void DisableComponents()
		{
			if (_header != null && _header.Visible)
				_header.Hide(true);
			
			base.DisableComponents();
		}
		
		protected override void OnWindowPanelsToggled(bool visible)
		{
			base.OnWindowPanelsToggled(visible);
			
			if (_header != null)
			{
				if (visible)
					_header.Show();
				else
					_header.Hide();
			}
		}
		
		private void OnHeaderClosed() => DispatchClosedEvent();
	}
}
