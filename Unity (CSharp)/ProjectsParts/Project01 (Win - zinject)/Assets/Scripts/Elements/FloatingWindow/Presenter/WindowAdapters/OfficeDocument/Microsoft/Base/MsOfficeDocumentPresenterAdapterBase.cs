using Cysharp.Threading.Tasks;
using Elements.FloatingWindow.Presenter.WindowAdapters.Base;
using Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components;
using Elements.Windows.Base.Data.Utils;
using Elements.Windows.Base.Presenter;
using UnityEngine;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.OfficeDocument.Microsoft.Base
{
    public abstract class MsOfficeDocumentPresenterAdapterBase<TWindowPresenter> : WindowPresenterAdapterBase<TWindowPresenter>
        where TWindowPresenter : IWindowPresenter
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