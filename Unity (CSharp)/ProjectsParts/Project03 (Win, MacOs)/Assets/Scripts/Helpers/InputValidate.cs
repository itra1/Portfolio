using System.Text.RegularExpressions;

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
	public static int Password(string value)
	{
		if (value.Length < 8 || value.Length > 20)
			return ErrorLenght;

		Regex regex = new Regex(@"^[A-Za-z0-9\-_]{8,20}$");
		if (regex.Matches(value).Count <= 0)
			return ErrorFormat;
		return Ok;
	}

	public static bool IsEmail(string value)
	{
		Regex regex = new Regex(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+.+.[A-Za-z]{2,4}$");
		return regex.Matches(value).Count > 0;
	}
	public static int Email(string value)
	{
		if (value.Length < 8 || value.Length > 20)
			return ErrorLenght;

		if (!IsEmail(value))
			return ErrorFormat;
		return Ok;
	}
	public static bool IsPhone(string value)
	{
		Regex regex = new Regex(@"^(?=[^7])\d{10,15}$");
		return regex.Matches(value).Count > 0;
	}
	public static int Phone(string value)
	{
		if (value.Length < 8 || value.Length > 20)
			return ErrorLenght;

		if (!IsPhone(value))
			return ErrorFormat;
		return Ok;
	}
}
