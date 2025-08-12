using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Providers.Saves.Adapters;
using Game.Providers.Saves.Data;
using Leguar.TotalJSON;
using Zenject;

namespace Game.Providers.Saves
{
	public class SaveProvider : ISaveProvider
	{
		private readonly DiContainer _container;
		private ISaveAdapter _saveAdapter;
		private bool _isLoadingProcess = false;
		private bool _isSaveProcess = false;

		private Dictionary<string, ISaveItem> _propertyes = new();

		public bool IsLoaded { get; private set; }

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
			if (!_isSaveProcess && _isLoadingProcess)
				return;

			_isSaveProcess = true;
			JSON data = new JSON();
			foreach (var item in _propertyes)
			{
				data.Add(item.Key, item.Value.Save());
			}

			string dataString = data.CreateString();

			await _saveAdapter.Save(dataString);
			_isSaveProcess = false;
		}

		public async UniTask Load()
		{
			_isLoadingProcess = true;

			var jsonString = await _saveAdapter.Load();
			var json = JSON.ParseString(jsonString);

			foreach (var item in _propertyes)
			{
				if (json.ContainsKey(item.Key))
					item.Value.Load(json.GetString(item.Key));
			}
			_isLoadingProcess = false;
		}

		public T GetProperty<T>() where T : ISaveItem
		{
			return (T) _propertyes[typeof(T).Name];
		}

		public async UniTask FirstLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			FindClasses();
			await Load();
		}
	}
}