using OfficeControl.Controllers;
using OfficeControl.Pipes.Common;
using OfficeControl.PowerPoints;
using OfficeControl.Pipes.Base;

namespace OfficeControl.Pipes.Packages
{
	public partial class PresentationPreviousSlide :IPackageProcess
	{
		public async Task<Package> Process()
		{
			PowerPoint officeApp = (PowerPoint)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp != null)
			{
				officeApp.SlidePreviousSet();
			}

			return new CommonOk();
		}
	}
}
