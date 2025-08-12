using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class ScreenCornerFloatingWindowSizeY : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public ScreenCornerFloatingWindowSizeY(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.ScreenCornerFloatingWindowSizeY)
        {
            _options = options;
        }
        
        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && float.TryParse(value, out var sizeY))
                _options.ScreenCornerFloatingWindowSizeY = sizeY;
        }
    }
}