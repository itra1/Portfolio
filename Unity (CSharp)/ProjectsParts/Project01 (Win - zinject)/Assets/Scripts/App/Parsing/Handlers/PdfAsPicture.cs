using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class PdfAsPicture : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public PdfAsPicture(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.PdfAsPicture)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsPdfRenderingAsPicture = true;
        }
    }
}