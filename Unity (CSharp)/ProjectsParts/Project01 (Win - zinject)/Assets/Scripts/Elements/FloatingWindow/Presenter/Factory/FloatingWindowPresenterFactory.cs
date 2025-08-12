using Core.Materials.Data;
using Elements.FloatingWindow.Presenter.WindowAdapters.Common.Factory;
using UnityEngine;
using Zenject;
using IPrefabProvider = UI.Prefabs.IPrefabProvider;

namespace Elements.FloatingWindow.Presenter.Factory
{
    public class FloatingWindowPresenterFactory : IFloatingWindowPresenterFactory
    {
        private readonly DiContainer _container;
        private readonly IPrefabProvider _prefabs;
        private readonly IWindowPresenterAdapterFactory _windowAdapterFactory;
        
        public FloatingWindowPresenterFactory(DiContainer container, 
            IPrefabProvider prefabs, 
            IWindowPresenterAdapterFactory windowAdapterFactory)
        {
            _container = container;
            _prefabs = prefabs;
            _windowAdapterFactory = windowAdapterFactory;
        }
        
        public IFloatingWindowPresenter Create(MaterialData material, RectTransform parent, bool isAdaptiveSizeRequired)
        {
            if (material == null)
            {
                Debug.LogError("Material is null when trying to create a floating window presenter");
                return default;
            }
			
            var prefab = _prefabs.GetPrefabOf<FloatingWindowPresenter>();
			
            if (prefab == null)
            {
                Debug.LogError("Prefab is not found by presenter type: FloatingWindowPresenter");
                return default;
            }
			
            if (parent == null)
            {
                Debug.LogError("An attempt was detected to assign a null parent to the FloatingWindowPresenter");
                return default;
            }
			
            var gameObject = Object.Instantiate(prefab, parent);
			
            gameObject.SetActive(false);
			
            var presenter = gameObject.GetComponent<IFloatingWindowPresenter>();
			
            _container.Inject(presenter);
			
            presenter.SetParentOnInitialize(parent);
            
            var windowAdapter = _windowAdapterFactory.Create(material, presenter.Content);
            
            if (windowAdapter == null)
                return default;
            
            windowAdapter.SetMaterial(material);
            
            if (isAdaptiveSizeRequired)
                presenter.AllowAdaptiveSize();
            
            return presenter.SetWindowAdapter(windowAdapter) ? presenter : default;
        }
    }
}