using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class RenderStreamingStun : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public RenderStreamingStun(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.RenderStreamStun)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsRenderStreamingStunUsing = true;
        }
    }
}