using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class ConfigManager
{
	public static Dictionary<string, string> Configs = new Dictionary<string, string>();
	
	public static void Load(){

		Configs = new Dictionary<string, string>();

		string filePath = Application.dataPath + "/../config.txt";

		if (!System.IO.File.Exists(filePath))
		{
			it.Logger.Log("No exists file " + filePath);
			return;
		}

		it.Logger.Log("Create component complete");
		//System.IO.File.ReadLines(Application.dataPath)
		StreamReader f = new StreamReader(filePath);


		while (!f.EndOfStream)
		{
			string s = f.ReadLine();

			if (s.Length < 4)
				continue;
			if (s.Substring(0, 2) == "//")
				continue;

			string[] property = s.Split(new string[1] { "=" }, System.StringSplitOptions.None);
			Configs.Add(property[0].Trim(), property[1].Trim());
		}
		f.Close();

	}
}
