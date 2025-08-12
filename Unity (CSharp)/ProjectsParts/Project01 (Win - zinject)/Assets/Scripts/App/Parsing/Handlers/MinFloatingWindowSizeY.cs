using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class MinFloatingWindowSizeY : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public MinFloatingWindowSizeY(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.MinFloatingWindowSizeY)
        {
            _options = options;
        }
        
        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && float.TryParse(value, out var sizeY))
                _options.MinFloatingWindowSizeY = sizeY;
        }
    }
}