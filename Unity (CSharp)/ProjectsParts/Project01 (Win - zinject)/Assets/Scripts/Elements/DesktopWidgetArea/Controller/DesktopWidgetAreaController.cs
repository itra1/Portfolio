using System.Threading;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Data;
using Core.Materials.Loading.Loader;
using Core.Materials.Loading.Loader.Consts;
using Core.Materials.Loading.Loader.Info;
using Core.Materials.Storage;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.DesktopWidgetArea.Presenter;
using Elements.Widgets.Common.Controller;
using Elements.Widgets.Common.Controller.Factory;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Elements.DesktopWidgetArea.Controller
{
	public class DesktopWidgetAreaController : IDesktopWidgetAreaController
	{
		private readonly IMaterialDataStorage _materials;
		private readonly IMaterialDataLoader _materialLoader;
		private readonly IWidgetControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly CancellationTokenSource _unloadCancellationTokenSource;

		private IDesktopWidgetAreaPresenter _presenter;
		private IWidgetController _child;
		
		public ContentAreaMaterialData AreaMaterial { get; }
		
		public DesktopWidgetAreaController(ContentAreaMaterialData areaMaterial,
			IMaterialDataStorage materials,
			IMaterialDataLoader materialLoader,
			IWidgetControllerFactory childControllerFactory, 
			IPresenterFactory presenterFactory)
		{
			AreaMaterial = areaMaterial;
			
			_materials = materials;
			_materialLoader = materialLoader;
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_unloadCancellationTokenSource = new CancellationTokenSource();
		}
		
		public async UniTask<bool> PreloadAsync(RectTransform parent)
		{
			_presenter = _presenterFactory.Create<DesktopWidgetAreaPresenter>(parent);
			
			if (_presenter == null)
			{
				Debug.LogError($"Failed to instantiate the DesktopWidgetAreaPresenter by area material {AreaMaterial}");
				return false;
			}
			
			if (!_presenter.SetMaterial(AreaMaterial))
				return false;
			
			_presenter.AlignToParent();
			
			var materialId = AreaMaterial.MaterialId;
			
			if (materialId == null)
			{
				Debug.LogError($"Missing material id in area material {AreaMaterial}");
				return false;
			}
			
			if (!await PreloadChildAsync(materialId.Value))
			{
				Debug.LogError($"Attempt to preload widget with material id {materialId.Value} is failed");
				return false;
			}
			
			_presenter.IdentifyOnlyChild();
			return true;
		}

		public bool Show()
		{
			if (_presenter == null || !_presenter.Show())
				return false;
			
			_child?.Show();
			return true;
		}
		
		public bool Hide()
		{
			_child?.Hide();
			return _presenter != null && _presenter.Hide();
		}

		public void Unload()
		{
			if (!_unloadCancellationTokenSource.IsCancellationRequested)
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
			}
			
			if (_child != null)
			{
				_child.Unload();
				_child = null;
			}

			if (_presenter != null)
			{
				_presenter.Unload();
				_presenter = null;
			}
		}
		
		private async UniTask<bool> PreloadChildAsync(ulong materialId)
		{
			var material = _materials.Get<WidgetMaterialData>(materialId) ?? await TryLoadWidgetMaterialAsync(materialId);
			
			if (material == null)
			{
				Debug.LogError($"Failed to get widget material by id {materialId}");
				return false;
			}
			
			var child = _childControllerFactory.Create(material);
			
			if (child == null || !await child.PreloadAsync(_presenter.Content)) 
				return false;
			
			_child = child;
			return true;
		}
		
		private async UniTask<WidgetMaterialData> TryLoadWidgetMaterialAsync(ulong materialId)
		{
			var info = new MaterialDataLoadingInfo(typeof(WidgetMaterialData), materialId, UrlPostfix.UnityOptimized);
			
			var result = await _materialLoader.LoadAsync(info);
			
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return null;
			
			if (!result.Success)
			{
				Debug.LogError($"Failed to load widget material by id {materialId}");
				return null;
			}
			
			if (!result.TryGetFirstMaterial<WidgetMaterialData>(out var material))
			{
				Debug.LogError($"No widget material was found in the loaded list of materials by requested material id {materialId}");
				return null;
			}
			
			return material;
		}
	}
}