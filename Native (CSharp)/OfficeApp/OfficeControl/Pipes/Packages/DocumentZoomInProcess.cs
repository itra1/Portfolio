using OfficeControl.Controllers;
using OfficeControl.Pipes.Common;
using OfficeControl.Pipes.Base;
using OfficeControl.Words;

namespace OfficeControl.Pipes.Packages
{
	public partial class DocumentZoomIn :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Word officeApp = (Word)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp != null)
			{
				officeApp.ZoomInSet();
			}

			return new CommonOk();
		}
	}
}
