using OfficeControl.Common;
using OfficeControl.Controllers;
using OfficeControl.Pipes.Common;
using OfficeControl.Pipes.Base;

namespace OfficeControl.Pipes.Packages
{
	public partial class CommonAppOpen :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Apps app = Apps.Instance;

			OfficeApplication uuidApp = app.OpenApp(Type,FilePath);

			OfficeApplication office = app.GetOfficeApplication(uuidApp.Uuid);

			return office.MakeOpenCompletePackage();

		}
	}
}
