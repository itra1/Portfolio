using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class ScreenCornerFloatingWindowSizeX : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public ScreenCornerFloatingWindowSizeX(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.ScreenCornerFloatingWindowSizeX)
        {
            _options = options;
        }
        
        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && float.TryParse(value, out var sizeX))
                _options.ScreenCornerFloatingWindowSizeX = sizeX;
        }
    }
}