using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class RenderStreamUrl : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public RenderStreamUrl(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.RenderStreamUrl)
        {
            _options = options;
        }
        
        protected override void Execute(string value)
        {
            base.Execute(value);
            _options.RenderStreamingUrl = value;
        }
    }
}