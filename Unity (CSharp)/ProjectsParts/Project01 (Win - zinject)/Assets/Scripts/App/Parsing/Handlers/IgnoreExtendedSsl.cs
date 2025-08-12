using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;
using Vuplex.WebView;

namespace App.Parsing.Handlers
{
    public class IgnoreExtendedSsl : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public IgnoreExtendedSsl(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.IgnoreExtendedSsl)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            StandaloneWebView.SetIgnoreCertificateErrors(true);
            _options.IsExtendedSslIgnored = true;
        }
    }
}