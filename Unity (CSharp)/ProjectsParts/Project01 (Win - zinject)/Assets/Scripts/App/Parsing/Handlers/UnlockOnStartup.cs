using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class UnlockOnStartup : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public UnlockOnStartup(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.UnlockOnStartup)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsUnlockedAtStart = true;
        }
    }
}