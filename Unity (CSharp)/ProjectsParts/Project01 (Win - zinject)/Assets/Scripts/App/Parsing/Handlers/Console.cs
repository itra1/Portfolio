using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class Console : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public Console(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.Console)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsConsoleEnabled = true;
        }
    }
}