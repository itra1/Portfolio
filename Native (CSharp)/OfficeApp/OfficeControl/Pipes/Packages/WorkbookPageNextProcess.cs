using OfficeControl.Controllers;
using OfficeControl.Excels;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	public partial class WorkbookPageNext :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Excel officeApp = (Excel)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp != null)
			{
				officeApp.PageNextSet();
			}

			return new CommonOk();
		}
	}
}
