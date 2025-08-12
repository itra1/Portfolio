using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Validator
{
	public static int Email(string str)
	{
		if (string.IsNullOrEmpty(str))
			return 0;

		string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
				+ @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
				+ @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

		Regex ValidEmailRegex = new Regex(validEmailPattern, RegexOptions.IgnoreCase);

		if (!ValidEmailRegex.IsMatch(str))
			return 1;

		return -1;
	}
	public static int Nickname(string str)
	{
		if (string.IsNullOrEmpty(str))
			return 0;

		if (str.Length < 3 || str.Length > 15)
			return 1;

		string validEmailPattern = @"^[a-zA-Z0-9_]+$";

		Regex ValidEmailRegex = new Regex(validEmailPattern, RegexOptions.IgnoreCase);

		if (!ValidEmailRegex.IsMatch(str))
			return 2;

		return -1;
	}
	public static int Password(string str)
	{
		if (string.IsNullOrEmpty(str))
			return 0;

		if (str.Length < 8 || str.Length > 20)
			return 1;


		string validEmailPattern = @"^[a-zA-Z0-9_\-%#!\.]+$";

		Regex ValidEmailRegex = new Regex(validEmailPattern, RegexOptions.IgnoreCase);

		if (!ValidEmailRegex.IsMatch(str))
			return 2;

		return -1;
	}

	public static int PhoneNumber(string str)
	{
		if (string.IsNullOrEmpty(str))
			return 0;

		if (str.Length < 8 || str.Length > 15)
			return 1;
		
		string validPhonePattern = @"[0-9]";
		
		Regex ValidEmailRegex = new Regex(validPhonePattern, RegexOptions.IgnoreCase);

		if (!ValidEmailRegex.IsMatch(str))
			return 2;

		return -1;
	}

	public static string PhoneCorrect(string value)
	{
		value = Regex.Replace(value, "^\\+", "");
		value = Regex.Replace(value, "^7", "");
		return value;
	}

}
