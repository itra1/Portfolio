using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class ScreenBorderLeft : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public ScreenBorderLeft(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.ScreenBorderLeft)
        {
            _options = options;
        }
        
        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && float.TryParse(value, out var left))
                _options.ScreenBorderLeft = left;
        }
    }
}