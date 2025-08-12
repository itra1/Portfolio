using OfficeControl.Controllers;
using OfficeControl.Excels;
using OfficeControl.Pipes.Common;
using OfficeControl.Pipes.Base;

namespace OfficeControl.Pipes.Packages
{
	public partial class WorkbookPageDown :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Excel officeApp = (Excel)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp != null)
			{
				officeApp.PageDownMove();
			}

			return new CommonOk();
		}
	}
}
