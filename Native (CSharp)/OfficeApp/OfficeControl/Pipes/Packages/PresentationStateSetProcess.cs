using OfficeControl.Controllers;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;
using OfficeControl.PowerPoints;

namespace OfficeControl.Pipes.Packages
{
	public partial class PresentationStateSet :IPackageProcess
	{
		public async Task<Package> Process()
		{
			PowerPoint officeApp = (PowerPoint)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp == null)
				return new CommonError();

			officeApp.SlideTargetSet(Slide);

			return new CommonOk();
		}
	}
}
