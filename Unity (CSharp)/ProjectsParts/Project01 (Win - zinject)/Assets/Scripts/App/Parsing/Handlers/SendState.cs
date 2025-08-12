using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class SendState : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public SendState(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.SendState)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsStateSendingAllowed = true;
        }
    }
}