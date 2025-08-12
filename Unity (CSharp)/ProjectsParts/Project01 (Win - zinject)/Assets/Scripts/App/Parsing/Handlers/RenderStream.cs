using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class RenderStream : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public RenderStream(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.RenderStream)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsRenderStreamingEnabled = true;
        }
    }
}