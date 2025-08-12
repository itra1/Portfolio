using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class OneThirdScreenFloatingWindowSizeY : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public OneThirdScreenFloatingWindowSizeY(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.OneThirdScreenFloatingWindowSizeY)
        {
            _options = options;
        }
        
        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && float.TryParse(value, out var sizeY))
                _options.OneThirdScreenFloatingWindowSizeY = sizeY;
        }
    }
}