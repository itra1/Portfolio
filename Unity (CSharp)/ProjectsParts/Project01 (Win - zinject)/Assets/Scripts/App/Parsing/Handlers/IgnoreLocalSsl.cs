using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class IgnoreLocalSsl : CommandLineFlagHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public IgnoreLocalSsl(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.IgnoreLocalSsl)
        {
            _options = options;
        }

        protected override void Execute()
        {
            base.Execute();
            _options.IsLocalSslIgnored = true;
        }
    }
}