using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	[PackageName(PackagesNames.DocumentZoomValue)]
	public partial class DocumentZoomValue :DocumentPackage
	{
		public int ZoomValue;

		public DocumentZoomValue(int zoom)
		{
			ZoomValue = zoom;
		}
	}
}
