using System.Collections.Generic;
using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class Server : CommandLineValuesHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public Server(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.Server, 1)
        {
            _options = options;
        }
        
        protected override void Execute(IReadOnlyList<string> values)
        {
            base.Execute(values);
            _options.CustomServer = values[0];
        }
    }
}