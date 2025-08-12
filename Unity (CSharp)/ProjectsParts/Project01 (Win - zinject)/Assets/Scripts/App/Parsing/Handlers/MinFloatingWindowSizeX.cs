using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class MinFloatingWindowSizeX : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public MinFloatingWindowSizeX(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.MinFloatingWindowSizeX)
        {
            _options = options;
        }
        
        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && float.TryParse(value, out var sizeX))
                _options.MinFloatingWindowSizeX = sizeX;
        }
    }
}