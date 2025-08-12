using UnityEngine;
using UnityEngine.Events;

namespace Core.Engine.Components.SaveGame
{
	public abstract class SaveProperty<T> :ISaveProperty<T>
	{
		public UnityEvent OnChange = new();

		protected T _value;
		public string Name => this.GetType().Name;

		public T Value
		{
			get => _value;
			set
			{
				_value = value;
				Save();
			}
		}

		public abstract T DefaultValue { get; }

		public SaveProperty()
		{
			Load();
		}

		public void Load()
		{

			if (!PlayerPrefs.HasKey(Name))
			{
				_value = DefaultValue;
				return;
			}
			AppLog.Log($"SaveProperty Load {Name} - {PlayerPrefs.GetString(Name)}");
			_value = (T)Newtonsoft.Json.JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(Name));

		}

		public void Save()
		{
			string str = Newtonsoft.Json.JsonConvert.SerializeObject(_value);
			AppLog.Log($"SaveProperty Save {Name} - {str}");
			PlayerPrefs.SetString(Name,str);
			PlayerPrefs.Save();
		}

	}
}
