using Core.Elements.Windows.OfficeDocument.Data.Web;
using Core.Materials.Attributes;
using Cysharp.Threading.Tasks;
using Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components;
using Elements.FloatingWindow.Presenter.WindowAdapters.WebView;
using Elements.Windows.Base.Data.Utils;
using Elements.Windows.OfficeDocument.Presenter.Web;
using UnityEngine;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.OfficeDocument.Web
{
    [MaterialData(typeof(WebOfficeDocumentMaterialData))]
    public class WebOfficeDocumentPresenterAdapter : WebViewPresenterAdapterBase<WebOfficeDocumentPresenter>
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
