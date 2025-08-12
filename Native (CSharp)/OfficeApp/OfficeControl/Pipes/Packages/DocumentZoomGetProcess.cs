using OfficeControl.Controllers;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;
using OfficeControl.Words;

namespace OfficeControl.Pipes.Packages
{
	public partial class DocumentZoomGet :IPackageProcess
	{
		public async Task<Package> Process()
		{
			Word officeApp = (Word)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp == null)
				return new CommonError();

			return new DocumentZoomValue(officeApp.ZoomCurrentGet());
		}
	}
}
