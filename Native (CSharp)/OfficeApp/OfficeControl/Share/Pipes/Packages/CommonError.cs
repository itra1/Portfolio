using OfficeControl.Pipes.Base;
using OfficeControl.Pipes.Common;

namespace OfficeControl.Pipes.Packages
{
	/// <summary>
	/// Простой ответ ОШИБКА
	/// </summary>
	[PackageName(PackagesNames.CommonError)]
	public partial class CommonError :CommonPackage
	{
		public string Error { get; private set; }

		public CommonError(string message = null)
		{
			if (!string.IsNullOrEmpty(message))
			{
				Error = message;
			}
			else
			{
				Error = "Error";
			}
		}
	}
}
