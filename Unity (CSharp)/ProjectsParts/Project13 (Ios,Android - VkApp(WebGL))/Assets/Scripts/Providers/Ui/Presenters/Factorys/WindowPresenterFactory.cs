using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Base;
using Game.Providers.Ui.Presenters.Base;
using Game.Providers.Ui.Settings;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Presenters.Factorys
{
	public class WindowPresenterFactory : IWindowPresenterFactory
	{
		private IWindowsPresentersSettings _settings;
		private DiContainer _diContainer;
		private Dictionary<Type, WindowPresenter> _prefabList = new();

		private List<WindowPresenter> _instances = new();

		public WindowPresenterFactory(DiContainer container, IWindowsPresentersSettings settings)
		{
			_diContainer = container;
			_settings = settings;

			LoadResources();
		}

		public async UniTask<T> GetInstance<T>()
		{
			var instanceType = _instances.Find(x => x.GetType() == typeof(T));
			try
			{
				if (instanceType != null)
					return instanceType.GetComponent<T>();
			}
			catch
			{
				throw new NullReferenceException("Не удается получить целевой компонент");
			}
			var prefab = _prefabList[typeof(T)];

			var instantiateOperation = MonoBehaviour.Instantiate(prefab);
			var inst = instantiateOperation;

			_diContainer.Inject(inst);

			var components = inst.GetComponentsInChildren<IInjection>(true);

			foreach (var item in components)
			{
				_diContainer.Inject(item);
			}

			return inst.GetComponent<T>();
		}

		private void LoadResources()
		{
			var presentersArray = Resources.LoadAll<WindowPresenter>(_settings.UiPresentersPrefabPath);

			for (int i = 0; i < presentersArray.Length; i++)
			{
				_prefabList.Add(presentersArray[i].GetType(), presentersArray[i]);
			}
		}
	}
}
