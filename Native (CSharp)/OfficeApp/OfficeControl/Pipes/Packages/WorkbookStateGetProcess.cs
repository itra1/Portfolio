using OfficeControl.Controllers;
using OfficeControl.Excels;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	public partial class WorkbookStateGet :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Excel officeApp = (Excel)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp == null)
				return new CommonError();

			return new WorkbookStateValue()
			{
				PageIndex = officeApp.PageIndexGet(),
				Zoom = officeApp.ZoomCurrentGet(),
				PagePositionX = officeApp.PagePositionXGet(),
				PagePositionY = officeApp.PagePositionYGet()
			};
		}
	}
}
