using App.Parsing.Handlers.Base;
using App.Parsing.Handlers.Consts;
using Core.Options;

namespace App.Parsing.Handlers
{
	public class MSOffice : CommandLineFlagHandler
	{
		private readonly IApplicationOptionsSetter _options;

		public MSOffice(IApplicationOptionsSetter options) : base(CommandLineArgumentNames.MSOffice)
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

			_options.IsMSOfficeUsed = true;

			Binds();
		}

		private void Binds()
		{
		}
	}
}