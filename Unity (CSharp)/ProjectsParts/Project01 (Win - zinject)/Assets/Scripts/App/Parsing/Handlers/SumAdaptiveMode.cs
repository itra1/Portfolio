using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class SumAdaptiveMode : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public SumAdaptiveMode(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.SumAdaptiveMode)
        {
            _options = options;
        }

        protected override void Execute()
        {
            base.Execute();
            _options.IsSumAdaptiveModeActive = true;
        }
    }
}