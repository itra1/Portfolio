
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
using System;

namespace Core.Engine.Components.SaveGame
{
	public class SaveGameProvider : ISaveGameProvider
	{
		private readonly string SaveKey = "AppSave";

		private Dictionary<string, ISaveItem> _propertyes = new();

		public bool IsInitiated { get; private set; } = false;

		public SaveGameProvider()
		{
			FindProperties();
		}

		private void FindProperties(){
			var types = from t in Assembly.GetExecutingAssembly().GetTypes()
									where t.IsClass 
										&& !t.IsAbstract
										&&	t.GetInterfaces().Contains(typeof(ISaveItem))
									select t;

			foreach (var t in types)
				_propertyes.Add(t.Name, (ISaveItem)Activator.CreateInstance(t));

			IsInitiated = true;
		}

		public ISaveItem GetProperty<T>(){
			return _propertyes[typeof(T).Name];
		}

	}
}