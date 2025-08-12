
using OfficeControl.Controllers;
using OfficeControl.Pipes.Common;
using OfficeControl.Pipes.Base;

namespace OfficeControl.Pipes.Packages
{
	public partial class CommonAppClose :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Apps.Instance.CloseApp(AppUuid);

			return new CommonOk();

		}
	}
}
