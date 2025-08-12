using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class LoadingIndicator : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public LoadingIndicator(IApplicationOptionsSetter options) 
            : base(CommandLineArgumentNames.LoadingIndicator)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsLoadingIndicatorEnabled = true;
        }
    }
}