using Base.Presenter;
using UnityEngine;
using Zenject;
using IPrefabProvider = UI.Prefabs.IPrefabProvider;

namespace Elements.Common.Presenter.Factory
{
	public class PresenterFactory : IPresenterFactory
	{
		private readonly DiContainer _container;
		private readonly IPrefabProvider _prefabs;
		private readonly INonRenderedContainer _nonRenderedContainer;
		
		public PresenterFactory(DiContainer container, IPrefabProvider prefabs, INonRenderedContainer nonRenderedContainer)
		{
			_container = container;
			_prefabs = prefabs;
			_nonRenderedContainer = nonRenderedContainer;
		}
		
		public TPresenter Create<TPresenter>(RectTransform parent) where TPresenter : PresenterBase, IPresenter
		{
			var prefab = _prefabs.GetPrefabOf<TPresenter>();
			
			if (prefab == null)
			{
				Debug.LogError($"Prefab is not found by presenter type: {typeof(TPresenter).Name}");
				return null;
			}
			
			if (parent == null)
			{
				Debug.LogError($"An attempt was detected to assign a null parent to the {typeof(TPresenter).Name}");
				return null;
			}
			
			var gameObject = Object.Instantiate(prefab, parent);
			
			gameObject.SetActive(false);
			
			var presenter = gameObject.GetComponent<TPresenter>();
			
			_container.Inject(presenter);
			
			if (presenter is INonRenderedCapable nonRenderedCapablePresenter)
				nonRenderedCapablePresenter.SetNonRenderedContainer(_nonRenderedContainer);
			
			presenter.SetParentOnInitialize(parent);
			
			return presenter;
		}
	}
}