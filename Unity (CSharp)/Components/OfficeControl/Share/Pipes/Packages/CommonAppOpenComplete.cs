using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	/// <summary>
	/// Успешное открытие файла
	/// </summary>
	[PackageName(PackagesNames.CommonOpenComplete)]
	public partial class CommonAppOpenComplete :CommonPackage
	{
		public string AppType;
	}
}
