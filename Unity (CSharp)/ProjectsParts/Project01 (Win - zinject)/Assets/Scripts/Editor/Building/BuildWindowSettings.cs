using System;
using System.Collections.Generic;
using Core.Materials.Attributes;
using Core.Materials.Parsing;
using Editor.Materials.Parsing;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Editor.Building
{
	public class BuildWindowSettings
	{
		[MaterialDataPropertyParse]
		public string BuildPath = Application.dataPath + "/../build/";
		[MaterialDataPropertyParse]
		public string TelegramBotKey = "bot1744139829:AAGdUcPAeo0-m0_8Xb02Kf65FKPA8j66lNU";
		[MaterialDataPropertyParse]
		public bool Archive = true;
		[MaterialDataPropertyParse]
		public bool RemoveSA = true;
		[MaterialDataPropertyParse]
		public bool ArchiveSA = false;
		[MaterialDataPropertyParse]
		public List<TelegramChat> Telegrams = new()
		{
			new TelegramChat { Title = "Work", Id = "-291477160", IsSend = true },
			new TelegramChat { Title = "Support", Id = "-789938879", IsSend = true } 
		};

		[Serializable]
		public class TelegramChat
		{
			[MaterialDataPropertyParse]
			public string Id;
			[MaterialDataPropertyParse]
			public string Title;
			[MaterialDataPropertyParse]
			public bool IsSend;
		}
		
		public static BuildWindowSettings Load()
		{
			var container = StaticContext.Container;
			var parsingHelper = container.TryResolve<IMaterialDataParsingHelper>();
			
			if (EditorPrefs.HasKey("windowEditor"))
			{
				var data = EditorPrefs.GetString("windowEditor");
				return parsingHelper.Parse<BuildWindowSettings>(data);
			}
			
			return new BuildWindowSettings();
		}
		
		public void Save()
		{
			var container = StaticContext.Container;
			var serializeHelper = container.TryResolve<IMaterialDataSerializeHelper>();
			
			var data = serializeHelper.Serialize(this);
			
			Debug.Log(data);
			EditorPrefs.SetString("windowEditor", data);
		}
	}
}
