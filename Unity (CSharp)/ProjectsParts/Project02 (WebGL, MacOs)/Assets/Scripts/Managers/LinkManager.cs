using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Garilla
{
	public class LinkManager : Singleton<LinkManager>
	{
		[SerializeField] private TextAsset _sourcesLinkData;
		private static Dictionary<string, string> _linkLibrary = new Dictionary<string, string>();
		private void Start()
		{
			ReadLinks();
		}

		private void ReadLinks()
		{
			string texts = _sourcesLinkData.text;
			var linkArray = texts.Split("\n");

			for (int i = 0; i < linkArray.Length; i++)
			{
				string s = linkArray[i];

				if (s.Length < 4)
					continue;
				if (s.Substring(0, 2) == "//")
					continue;
				if (s.Substring(0, 1) == ",")
					continue;

				string[] property = s.Split(new string[1] { "," }, System.StringSplitOptions.None);
				_linkLibrary.Add(property[0].Trim(), property[2].Trim());
			}
		}

		private void PrintLinks()
		{
			foreach (var key in _linkLibrary.Keys)
			{
				it.Logger.Log(key + " : " + _linkLibrary[key]);
			}
		}

		public static void OpenUrl(string key)
		{
			if (!_linkLibrary.ContainsKey(key)) return;

			Application.OpenURL(_linkLibrary[key]);
		}
	}
}