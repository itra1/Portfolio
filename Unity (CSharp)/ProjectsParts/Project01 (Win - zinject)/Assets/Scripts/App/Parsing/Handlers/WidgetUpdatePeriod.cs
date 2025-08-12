using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class WidgetUpdatePeriod : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public WidgetUpdatePeriod(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.WidgetUpdatePeriod)
        {
            _options = options;
        }

        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var period))
                _options.WidgetUpdatePeriod = period;
        }
    }
}