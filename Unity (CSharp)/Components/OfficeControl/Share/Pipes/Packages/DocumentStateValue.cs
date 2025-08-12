using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	[PackageName(PackagesNames.DocumentStateValue)]
	public partial class DocumentStateValue :DocumentPackage
	{
		public int Scroll;
		public int Zoom;

		public DocumentStateValue(int zoom,int scroll)
		{
			Zoom = zoom;
			Scroll = scroll;
		}
	}
}
