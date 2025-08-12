using Core.Engine.App.Base.Attributes.Defines;
using Core.Engine.Helpers;
using Core.Engine.Installers;

namespace Core.Engine.Services.WVService
{
#if UNITY_EDITOR
	public class WebViewServiceDefine :IToggleDefine
	{
		public string Symbol => "WEBVIEW_SERVICE";

		public string Description => "WebView service";

		public void AfterEnable()
		{
			ContextHelpers.AddMonoInstaller<WebViewInstaller>("Assets/Core/Resources/ProjectContext.prefab");
		}

		public void AfterDisable()
		{
			ContextHelpers.RemoveMonoInstaller<WebViewInstaller>("Assets/Core/Resources/ProjectContext.prefab");
		}
	}
#endif
}