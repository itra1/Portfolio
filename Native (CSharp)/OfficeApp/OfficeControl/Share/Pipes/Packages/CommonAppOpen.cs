using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	/// <summary>
	/// Запрос открытия файла
	/// </summary>
	[PackageName(PackagesNames.CommonOpen)]
	public partial class CommonAppOpen :CommonPackage
	{
		public string Type = "";
		public string FilePath = "";
	}
}
