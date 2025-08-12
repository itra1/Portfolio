using System;
using System.Linq;

using UnityEngine;


public class CommandLineController
{
	public static void PrintArguments()
	{

		string[] arguments = Environment.GetCommandLineArgs();
		string printArguments = "PrintArguments:";
		for(int i = 0; i < arguments.Length; i++)
		{
			printArguments += $"\n{i}: {arguments[i]}";
		}
		it.Logger.Log(printArguments);
	}

	public static void ConfirmArguments(){
		AppConfig.IsLog = GetLog();
		//PrintArguments();

		AppConfig.SessinId = GetSession();
		AppConfig.TableExe = GetIdTableFromExe();
		AppConfig.IsDevApp = IsDevApp();
		AppConfig.DevServer = IsDevelopServer();
		AppConfig.CustomServer = GetCustomServerApi();
		AppConfig.CustomServerWS = GetCustomServerWS();
	}

	public static ulong? GetIdTableFromExe()
	{
		//#if UNITY_EDITOR
		//		return 747;
		//#endif

		string[] arguments = Environment.GetCommandLineArgs();
		foreach (var arg in arguments)
		{
			if (arg.Contains("tableId"))
			{
				var index = arg.LastIndexOf("=", StringComparison.Ordinal);
				var idTable = arg.Substring(index + 1);
				ulong.TryParse(idTable, out ulong id);
				return id;
			}
		}

		return null;
	}

	public static string GetCustomServerApi()
	{
		string[] arguments = Environment.GetCommandLineArgs();
		foreach (var arg in arguments)
		{
			if (arg.Contains("serverApi"))
			{
				var index = arg.LastIndexOf("=", StringComparison.Ordinal);
				return arg.Substring(index + 1);
			}
		}

		return null;
	}

	public static string GetCustomServerWS()
	{
		string[] arguments = Environment.GetCommandLineArgs();
		foreach (var arg in arguments)
		{
			if (arg.Contains("serverWS"))
			{
				var index = arg.LastIndexOf("=", StringComparison.Ordinal);
				return arg.Substring(index + 1);
			}
		}

		return null;
	}

	public static string GetServersData()
	{
#if UNITY_EDITOR
		return "eyJlcnJvciI6MCwic2VydmVycyI6eyJnYW1lIjpbImh0dHBzOlwvXC9zcnYuZ2FyaWxsYXBva2VyLmNvbSJdLCJkZWxpdmVyeSI6WyJodHRwczpcL1wvZ2FyaWxsYXBva2VyLmNsdWIiXSwiYXBwcyI6WyJodHRwczpcL1wvZ2FyaWxsYS1hcHBzLmNvbSJdfX0=";
#endif

		string[] arguments = Environment.GetCommandLineArgs();

		for (int i = 0; i < arguments.Length; i++)
		{
			if (arguments[i] == "-servData")
			{
				return arguments[++i];
			}
		}
		return "";
	}
	public static bool IsDevelopServer()
	{
		string[] arguments = Environment.GetCommandLineArgs();

		for (int i = 0; i < arguments.Length; i++)
		{
			if (arguments[i] == "-gameDevelop")
			{
				return true;
			}
		}
		return false;
	}

	public static string GetPassword()
	{
		string[] arguments = Environment.GetCommandLineArgs();
		foreach (var arg in arguments)
		{
			if (arg.Contains("password"))
			{
				var index = arg.LastIndexOf("=", StringComparison.Ordinal);
				var password = arg.Substring(index + 1);
				return password;
			}
		}

		return "";
	}
	public static int? GetSession()
	{

		string[] arguments = Environment.GetCommandLineArgs();
		foreach (var arg in arguments)
		{
			if (arg.Contains("session"))
			{
				var index = arg.LastIndexOf("=", StringComparison.Ordinal);
				var resultString = arg.Substring(index + 1);
				int.TryParse(resultString, out int id);
				return id;
			}
		}

		return null;
	}
	public static bool ExistsArgumentExe(string argumentName)
	{
		string[] arguments = Environment.GetCommandLineArgs();
		foreach (var arg in arguments)
		{
			if (arg.Contains(argumentName))
			{
				return true;
			}
		}

		return false;
	}
	//YXJ0ZW1fcmVm
	public static string GetPromoExe()
	{
		//#if UNITY_EDITOR
		//		return "YXJ0ZW1fcmVm";
		//#endif
		string[] arguments = Environment.GetCommandLineArgs();

		for (int i = 0; i < arguments.Length; i++)
		{

			if (arguments[i] == "-promo")
			{
				return arguments[++i];
			}

		}
		return "";
	}

	public static string GetStagFromExe()
	{
		string[] arguments = Environment.GetCommandLineArgs();

		for (int i = 0; i < arguments.Length; i++)
		{

			if (arguments[i] == "-stag")
			{
				return arguments[++i];
			}

		}
		return "";
	}

	public static bool GetLog()
	{
		string[] arguments = Environment.GetCommandLineArgs();

		for (int i = 0; i < arguments.Length; i++)
		{

			if (arguments[i] == "-log")
			{
				return true;
			}

		}
		return false;
	}

	public static string GetLauncherPath()
	{
		string[] arguments = Environment.GetCommandLineArgs();

		for (int i = 0; i < arguments.Length; i++)
		{

			if (arguments[i] == "-sourceLauncher")
			{
				return arguments[i + 1];
			}

		}

		return null;
	}

	public static bool IsDevApp()
	{
		string[] arguments = Environment.GetCommandLineArgs();

		for (int i = 0; i < arguments.Length; i++)
		{

			if (arguments[i] == "-devApp")
			{
				return true;
			}

		}

		return false;
	}

}
