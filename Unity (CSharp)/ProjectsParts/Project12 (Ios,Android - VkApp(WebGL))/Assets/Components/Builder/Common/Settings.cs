using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Builder.Common
{
	public class Settings
	{
		public string BuildPath = Application.dataPath + "/../build/";
		public bool Archivate = true;
		public string ArchivePath = Application.dataPath + "/Archive/";
		public string TelegramBotKey = "bot1744139829:AAGdUcPAeo0-m0_8Xb02Kf65FKPA8j66lNU";
		public string TelegramGroup = "-000000000";
		public bool Archive = true;
		public List<TelegramChat> Telegrams = new()
		{
			new TelegramChat { Title = "Work", Id = "-291477160", IsSend = true },
			new TelegramChat { Title = "Support", Id = "-789938879", IsSend = true }
		};

		[Serializable]
		public class TelegramChat
		{
			public string Id;
			public string Title;
			public bool IsSend;
		}

		public static Settings Load()
		{
			_ = StaticContext.Container;

			if (EditorPrefs.HasKey("windowEditor"))
			{
				var data = EditorPrefs.GetString("windowEditor");
				return Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(data);
			}

			return new Settings();
		}

		public void Save()
		{
			_ = StaticContext.Container;

			var data = Newtonsoft.Json.JsonConvert.SerializeObject(this);

			Debug.Log(data);
			EditorPrefs.SetString("windowEditor", data);
		}
	}
}
