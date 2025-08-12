using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class ScreenBorderDown : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public ScreenBorderDown(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.ScreenBorderDown)
        {
            _options = options;
        }
        
        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && float.TryParse(value, out var down))
                _options.ScreenBorderDown = down;
        }
    }
}