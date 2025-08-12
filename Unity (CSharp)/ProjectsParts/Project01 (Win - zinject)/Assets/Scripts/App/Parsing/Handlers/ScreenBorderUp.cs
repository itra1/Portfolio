using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class ScreenBorderUp : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public ScreenBorderUp(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.ScreenBorderUp)
        {
            _options = options;
        }
        
        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && float.TryParse(value, out var up))
                _options.ScreenBorderUp = up;
        }
    }
}