using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	[PackageName(PackagesNames.DocumentScrollValue)]
	public partial class DocumentScrollValue :DocumentPackage
	{
		public int Value;

		public DocumentScrollValue(int val)
		{
			Value = val;
		}
	}
}
