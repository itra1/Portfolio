using OfficeControl.Controllers;
using OfficeControl.Pipes.Common;
using OfficeControl.Pipes.Base;

namespace OfficeControl.Pipes.Packages
{
	public partial class CommonQuit :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Apps.Instance.CloseAll();
			Environment.Exit(0);
			return new CommonOk();
		}
	}
}
