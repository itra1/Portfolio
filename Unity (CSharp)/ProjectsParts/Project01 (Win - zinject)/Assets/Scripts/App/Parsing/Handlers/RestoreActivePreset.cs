using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class RestoreActivePreset : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public RestoreActivePreset(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.RestoreActivePreset)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsPresetRestoredAtStart = true;
        }
    }
}