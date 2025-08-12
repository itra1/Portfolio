using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class DevServer : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public DevServer(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.DevServer)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsDevServerEnabled = true;
        }
    }
}