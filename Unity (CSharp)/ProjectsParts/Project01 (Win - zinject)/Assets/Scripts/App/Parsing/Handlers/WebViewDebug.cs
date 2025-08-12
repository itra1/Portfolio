using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Vuplex.WebView;

namespace App.Parsing.Handlers
{
    public class WebViewDebug : CommandLineFlagHandler
    {
        public WebViewDebug() : base(CommandLineArgumentNames.WebViewDebug) { }
        
        protected override void Execute()
        {
            base.Execute();
            StandaloneWebView.EnableRemoteDebugging(30001);
        }
    }
}