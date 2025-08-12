using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class DefaultCustomFloatingWindowSizeX : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public DefaultCustomFloatingWindowSizeX(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.DefaultCustomFloatingWindowSizeX)
        {
            _options = options;
        }
        
        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && float.TryParse(value, out var sizeX))
                _options.DefaultCustomFloatingWindowSizeX = sizeX;
        }
    }
}