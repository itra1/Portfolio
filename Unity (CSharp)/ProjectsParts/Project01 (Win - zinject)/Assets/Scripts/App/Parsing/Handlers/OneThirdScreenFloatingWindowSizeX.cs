using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class OneThirdScreenFloatingWindowSizeX : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public OneThirdScreenFloatingWindowSizeX(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.OneThirdScreenFloatingWindowSizeX)
        {
            _options = options;
        }
        
        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && float.TryParse(value, out var sizeX))
                _options.OneThirdScreenFloatingWindowSizeX = sizeX;
        }
    }
}