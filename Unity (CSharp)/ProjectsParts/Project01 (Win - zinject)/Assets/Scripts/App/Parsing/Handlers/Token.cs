using System.Collections.Generic;
using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
    public class Token : CommandLineValuesHandler
    {
        private readonly IApplicationOptionsSetter _options;
        
        public Token(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.Token, 1)
        {
            _options = options;
        }
        
        protected override void Execute(IReadOnlyList<string> values)
        {
            base.Execute(values);
            _options.ServerToken = values[0];
        }
    }
}