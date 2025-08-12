using OfficeControl.Controllers;
using OfficeControl.Pipes.Common;
using OfficeControl.Pipes.Base;
using OfficeControl.Words;

namespace OfficeControl.Pipes.Packages
{
	public partial class DocumentZoomOut :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Word officeApp = (Word)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp != null)
			{
				officeApp.ZoomOutSet();
			}

			return new CommonOk();
		}
	}
}
