using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	[PackageName(PackagesNames.WorkbookStateSet)]
	public partial class WorkbookStateSet :DocumentPackage
	{
		public int PagePositionX;
		public int PagePositionY;
		public int PageIndex;
		public double Zoom;

	}
}
