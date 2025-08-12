using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	[PackageName(PackagesNames.DocumentScrollSet)]
	public partial class DocumentScrollSet :DocumentPackage
	{
		public int Value;

		public DocumentScrollSet(int val)
		{
			Value = val;
		}
	}
}
