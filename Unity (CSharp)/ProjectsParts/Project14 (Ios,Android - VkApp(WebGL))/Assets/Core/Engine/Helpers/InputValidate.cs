using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Core.Engine.Helpers
{
/// <summary>
/// Валидация ввода игроков
/// </summary>
	public static class InputValidate
	{
		public const int Ok = 0;
		public const int ErrorLenght = 1;
		public const int ErrorFormat = 2;

		public static int UserName(string value)
		{
			if (value.Length < 3 || value.Length > 20)
				return ErrorLenght;

			Regex regex = new Regex(@"^[A-Za-z0-9\. \-_]{3,20}$");
			if (regex.Matches(value).Count <= 0)
				return ErrorFormat;
			return Ok;
		}
	}
}
