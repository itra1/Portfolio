using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class ScreenBorderRight : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public ScreenBorderRight(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.ScreenBorderRight)
        {
            _options = options;
        }
        
        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && float.TryParse(value, out var right))
                _options.ScreenBorderRight = right;
        }
    }
}