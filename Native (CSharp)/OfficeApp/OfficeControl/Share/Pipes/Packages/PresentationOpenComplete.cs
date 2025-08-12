using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	[PackageName(PackagesNames.PresentationOpenComplete)]
	public partial class PresentationOpenComplete :PresentationPackage
	{
		public string WindowName;
		public int ProcessId;
	}

}
