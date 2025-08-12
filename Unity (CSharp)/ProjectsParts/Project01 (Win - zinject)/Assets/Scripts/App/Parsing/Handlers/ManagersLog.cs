using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class ManagersLog : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public ManagersLog(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.ManagersLog)
        {
            _options = options;
        }
        
        protected override void Execute()
        {
            base.Execute();
            _options.IsManagersLogEnabled = true;
        }
    }
}