using OfficeControl.Controllers;
using OfficeControl.Pipes.Common;
using OfficeControl.PowerPoints;
using OfficeControl.Pipes.Base;

namespace OfficeControl.Pipes.Packages
{
	public partial class PresentationNextSlide :IPackageProcess
	{
		public async Task<Package> Process()
		{
			PowerPoint officeApp = (PowerPoint)Apps.Instance.GetOfficeApplication(AppUuid);

			if (officeApp != null)
			{
				officeApp.SlideNextSet();
			}

			return new CommonOk();

		}
	}
}
