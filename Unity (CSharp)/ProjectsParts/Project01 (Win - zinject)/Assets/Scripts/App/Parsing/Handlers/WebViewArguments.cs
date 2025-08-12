using System.Collections.Generic;
using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Vuplex.WebView;

namespace App.Parsing.Handlers
{
    public class WebViewArguments : CommandLineValuesHandler
    {
        public WebViewArguments() : base(CommandLineArgumentNames.WebViewArguments, 18) { }
        
        protected override void Execute(IReadOnlyList<string> values)
        {
            base.Execute(values);
            
            foreach (var value in values)
                StandaloneWebView.SetCommandLineArguments(value);
        }
    }
}