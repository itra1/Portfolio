using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	[PackageName(PackagesNames.PresentationStateSet)]
	public partial class PresentationStateSet :DocumentPackage
	{
		public int Slide;
	}
}
