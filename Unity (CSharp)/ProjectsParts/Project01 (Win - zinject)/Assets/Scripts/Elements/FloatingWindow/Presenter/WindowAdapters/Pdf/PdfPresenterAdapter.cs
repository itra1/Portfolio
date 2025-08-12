using Base.Presenter;
using Core.Elements.Windows.Pdf.Data;
using Core.Materials.Attributes;
using Cysharp.Threading.Tasks;
using Elements.FloatingWindow.Presenter.WindowAdapters.Base;
using Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components;
using Elements.Windows.Base.Data.Utils;
using Elements.Windows.Pdf.Presenter;
using UnityEngine;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Pdf
{
    [MaterialData(typeof(PdfMaterialData))]
    public class PdfPresenterAdapter : WindowPresenterAdapterBase<PdfPresenter>, INonRenderedCapable
    {
        [SerializeField] private WindowHeader _header;
        
        protected WindowHeader Header => _header;
        
        public void SetNonRenderedContainer(INonRenderedContainer container) => 
            Adaptee.SetNonRenderedContainer(container);
        
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
