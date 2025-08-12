//#define PRINT_LOG // вывод лога

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Saves.Adapters;
using Game.Scripts.Providers.Saves.Data;
using Leguar.TotalJSON;
using Zenject;

namespace Game.Scripts.Providers.Saves
{
	public class SaveProvider : ISaveProvider
	{
		private ISaveAdapter _saveAdapter;
		private readonly DiContainer _container;
		private Dictionary<string, ISaveItem> _propertyes = new();
		private bool _isLoadingProcess = false;
		private bool _isSaveProcess = false;
		private bool _saveChangeData = false;

		public SaveProvider(DiContainer container)
		{
			_container = container;
			_saveAdapter = new FileSaveAdapter();
		}

		private void FindClasses()
		{
			var types = from t in Assembly.GetExecutingAssembly().GetTypes()
									where t.IsClass
										&& !t.IsAbstract
										&& t.GetInterfaces().Contains(typeof(ISaveItem))
									select t;

			foreach (var t in types)
			{
				var inst = (ISaveItem) Activator.CreateInstance(t);
				_container.Inject(inst);
				_propertyes.Add(t.Name, inst);
			}
		}

		public async UniTask Save()
		{
			_saveChangeData = true;
			if (!_isSaveProcess && _isLoadingProcess)
				return;

			_isSaveProcess = true;
			JSON data = new JSON();
			foreach (var item in _propertyes)
			{
				data.Add(item.Key, item.Value.Save());
			}
			var saveData = data.CreateString();
			Log($"Save {saveData}");
			_saveChangeData = false;
			await _saveAdapter.Save(saveData);
			_isSaveProcess = false;

			if (_saveChangeData)
				_ = Save();
		}

		public async UniTask Load()
		{
			_isLoadingProcess = true;
			var jsonString = await _saveAdapter.Load();
			var json = JSON.ParseString(jsonString);

			foreach (var item in _propertyes)
			{
				if (json.ContainsKey(item.Key))
				{
					var loadData = json.GetString(item.Key);
					Log($"Load {item.Key} {loadData}");
					item.Value.Load(loadData);
				}
			}
			_isLoadingProcess = false;
		}

		public T GetProperty<T>() where T : ISaveItem
		{
			return (T) _propertyes[typeof(T).Name];
		}

		public async UniTask StartAppLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			FindClasses();
			await Load();
		}

		private void Log(string log)
		{
#if PRINT_LOG
			AppLog.Log(log);
#endif
		}

		public void CrearProgress()
		{
			_ = _saveAdapter.Save((new JSON()).CreateString());
		}
	}
}