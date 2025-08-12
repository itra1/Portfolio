using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class Fps : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public Fps(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.Fps)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsFpsCounterEnabled = true;
        }
    }
}