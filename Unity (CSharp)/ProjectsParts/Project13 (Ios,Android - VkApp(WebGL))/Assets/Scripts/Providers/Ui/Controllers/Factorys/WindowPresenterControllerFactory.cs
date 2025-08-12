using System;
using System.Collections.Generic;
using System.Linq;
using Game.Providers.Ui.Attributes;
using Game.Providers.Ui.Controllers.Base;
using ModestTree;
using Zenject;

namespace Game.Providers.Ui.Controllers.Factorys
{
	public class WindowPresenterControllerFactory : IWindowPresenterControllerFactory, IInitializable
	{
		private Dictionary<IUiControllerAttribute, IWindowPresenterController> _instancesList = new();
		private DiContainer _diContainer;

		public WindowPresenterControllerFactory(DiContainer diContainer)
		{
			_diContainer = diContainer;

		}

		public void Initialize()
		{
			ReadControllers();
		}

		private void ReadControllers()
		{
			_instancesList.Clear();

			Type parentType = typeof(WindowPresenterControllerBase);

			var childTypes = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(s => s.GetTypes())
					.Where(type => type.IsSubclassOf(parentType) && !type.IsAbstract).ToList();

			foreach (var item in childTypes)
			{
				var attribut = item.GetAttribute<UiControllerAttribute>();

				if (attribut == null)
					continue;

				var instance = Activator.CreateInstance(item) as IWindowPresenterController;

				_diContainer.Inject(instance);

				_instancesList.Add(attribut, instance);
			}
		}

		public T GetInstance<T>()
		{
			foreach (var item in _instancesList.Values)
			{
				if (item.GetType() == typeof(T))
					return (T) item;
			}
			return default(T);
		}

		public IWindowPresenterController GetInstance(string presenterName)
		{
			foreach (var item in _instancesList.Keys)
			{
				if (item.PresenterName == presenterName)
					return _instancesList[item];
			}
			return null;
		}
	}
}
