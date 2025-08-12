using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
	public class QtBrowser : CommandLineFlagHandler
	{
		private readonly IApplicationOptionsSetter _options;

		public QtBrowser(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.QtBrowser)
		{
			_options = options;
		}

		public override void ExecuteForce()
		{
			Execute();
		}

		protected override void Execute()
		{
			base.Execute();
			_options.QtBrowser = true;
		}
	}
}
