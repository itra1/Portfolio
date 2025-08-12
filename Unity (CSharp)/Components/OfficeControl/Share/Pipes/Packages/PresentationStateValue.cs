using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	[PackageName(PackagesNames.PresentationStateValue)]
	public partial class PresentationStateValue :DocumentPackage
	{
		public int Slide;
	}
}
