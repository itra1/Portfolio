using OfficeControl.Controllers;
using OfficeControl.Excels;
using OfficeControl.Pipes.Common;
using OfficeControl.Pipes.Base;

namespace OfficeControl.Pipes.Packages
{
	public partial class WorkbookZoomIn :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Excel officeApp = (Excel)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp != null)
			{
				officeApp.ZoomInSet();
			}

			return new CommonOk();
		}
	}
}
