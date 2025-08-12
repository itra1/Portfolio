using OfficeControl.Controllers;
using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;
using OfficeControl.PowerPoints;

namespace OfficeControl.Pipes.Packages
{
	public partial class PresentationSlideGet :IPackageProcess
	{
		public async Task<Package> Process()
		{
			PowerPoint officeApp = (PowerPoint)Apps.Instance.GetOfficeApplication(AppUuid);

			int page = officeApp != null ? officeApp.SlideCurrentGet() : 0;
			int totalpage = officeApp != null ? officeApp.SlideTotalGet() : 0;

			return new PresentationSlideResult(page, totalpage);
		}
	}
}
