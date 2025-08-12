using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class MakePreview : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public MakePreview(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.MakePreview)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsPreviewEnabled = true;
        }
    }
}