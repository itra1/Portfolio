using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	/// <summary>
	/// Простой ответ УСПЕШНО
	/// </summary>
	[PackageName(PackagesNames.CommonOk)]
	public partial class CommonOk :CommonPackage
	{
		public string Ok => "Ok";
	}
}
