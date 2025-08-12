using OfficeControl.Controllers;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;
using OfficeControl.Words;

namespace OfficeControl.Pipes.Packages
{
	public partial class DocumentStateSet :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Word officeApp = (Word)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp == null)
				return new CommonError();

			officeApp.ZoomCurrentSet(Zoom);
			officeApp.ListPositionSet(Scroll);

			return new CommonOk();
		}
	}
}
