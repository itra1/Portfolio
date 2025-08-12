using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class OnTopByDefault : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public OnTopByDefault(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.OnTopOnDefault)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsOnTopByDefault = true;
        }
    }
}