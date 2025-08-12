using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	[PackageName(PackagesNames.WorkbookStateValue)]
	public partial class WorkbookStateValue :DocumentPackage
	{
		public int PagePositionX;
		public int PagePositionY;
		public int PageIndex;
		public double Zoom;
	}
}
