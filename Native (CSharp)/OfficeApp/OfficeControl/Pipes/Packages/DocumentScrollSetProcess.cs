using OfficeControl.Controllers;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;
using OfficeControl.Words;

namespace OfficeControl.Pipes.Packages
{
	public partial class DocumentScrollSet :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Word officeApp = (Word)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp == null)
				return new CommonError();

			officeApp.ListPositionSet(Value);

			return new CommonOk();
		}

	}
}
