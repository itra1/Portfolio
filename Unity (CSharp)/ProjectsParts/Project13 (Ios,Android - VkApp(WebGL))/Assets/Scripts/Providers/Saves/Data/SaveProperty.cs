using System;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Providers.Saves.Data
{
	[Serializable]
	public abstract class SaveProperty<T> : ISaveProperty<T>
	{
		public UnityEvent OnChange = new();

		protected T _value;
		protected SignalBus _signalBus;

		public string Name => this.GetType().Name;

		public T Value
		{
			get => _value ??= DefaultValue;
			set
			{
				_value = value;
				_ = Save();
			}
		}

		public abstract T DefaultValue { get; }

		public SaveProperty()
		{
			//Load();
		}

		[Inject]
		public void Constructor(SignalBus signalbus)
		{
			_signalBus = signalbus;
		}

		public void Load(string data)
		{

			//if (!PlayerPrefs.HasKey(Name))
			//{
			//	AppLog.Log($"SaveProperty Load default {Name}");
			//	_value = DefaultValue;
			//	return;
			//}
			//var dataString = PlayerPrefs.GetString(Name);
			//AppLog.Log($"SaveProperty Load {Name} - {dataString}");
			try
			{
				_value = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);
			}
			catch (System.Exception ex)
			{
				PlayerPrefs.DeleteKey(Name);
				Debug.LogError($"Error parsing save data {Name} set as default. Error {ex.Message} stack {ex.StackTrace}");
				_value = DefaultValue;
			}
		}

		public string Save()
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(_value);
			//var str = Newtonsoft.Json.JsonConvert.SerializeObject(_value);
			//AppLog.Log($"SaveProperty Save {Name} - {str}");
			//PlayerPrefs.SetString(Name, str);
			//PlayerPrefs.Save();
			//AfterSave();
		}

		protected virtual void AfterSave() { }
	}
}