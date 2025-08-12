using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	[PackageName(PackagesNames.DocumentZoomSet)]
	public partial class DocumentZoomSet :DocumentPackage
	{
		public int ZoomValue;

		public DocumentZoomSet(int zoom)
		{
			ZoomValue = zoom;
		}
	}
}
