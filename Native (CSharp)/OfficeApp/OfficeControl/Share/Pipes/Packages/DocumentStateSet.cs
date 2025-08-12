using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	[PackageName(PackagesNames.DocumentStateSet)]
	public partial class DocumentStateSet :DocumentPackage
	{
		public int Scroll;
		public int Zoom;

		public DocumentStateSet(int zoom,int scroll)
		{
			Zoom = zoom;
			Scroll = scroll;
		}
	}
}
