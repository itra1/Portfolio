using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class SumAdaptiveColumn : CommandLinePairHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public SumAdaptiveColumn(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.SumAdaptiveColumn)
        {
            _options = options;
        }

        protected override void Execute(string value)
        {
            base.Execute(value);
            
            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var column))
                _options.SumAdaptiveModeColumn = column;
        }
    }
}