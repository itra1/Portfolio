using OfficeControl.Controllers;
using OfficeControl.Excels;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	public partial class WorkbookStateSet :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Excel officeApp = (Excel)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp == null)
				return new CommonError();

			officeApp.PageIndexSet(PageIndex);
			officeApp.ZoomCurrentSet(Zoom);
			officeApp.PagePositionSet(PagePositionX,PagePositionY);

			return new CommonOk();
		}

	}
}
