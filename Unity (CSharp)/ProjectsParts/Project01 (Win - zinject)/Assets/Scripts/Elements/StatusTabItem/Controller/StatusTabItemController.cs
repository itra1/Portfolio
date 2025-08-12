using System;
using System.Reflection;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.Windows.Base.Data;
using Core.Elements.Windows.Base.Data.Attributes;
using Core.Materials.Data;
using Core.Materials.Storage;
using Core.Messages;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.StatusTabItem.Presenter;
using Elements.Windows.Common.Controller;
using Elements.Windows.Common.Controller.Factory;
using Preview;
using ScreenStreaming;
using UnityEngine;

namespace Elements.StatusTabItem.Controller
{
	public class StatusTabItemController : IStatusTabItemController
	{
		private readonly IMaterialDataStorage _materials;
		private readonly IWindowControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IPreviewProvider _previewProvider;
		private readonly IScreenStreamingController _screenStreaming;
		private readonly CancellationTokenSource _unloadCancellationTokenSource;
		
		private IStatusTabItemPresenter _presenter;
		private IWindowController _child;
		
		public ContentAreaMaterialData AreaMaterial { get; }
		
		public StatusTabItemController(ContentAreaMaterialData areaMaterial,
			IMaterialDataStorage materials,
			IWindowControllerFactory childControllerFactory, 
			IPresenterFactory presenterFactory,
			IPreviewProvider previewProvider,
			IScreenStreamingController screenStreaming)
		{
			AreaMaterial = areaMaterial;
			
			_materials = materials;
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_previewProvider = previewProvider;
			_screenStreaming = screenStreaming;
			_unloadCancellationTokenSource = new CancellationTokenSource();
		}
		
		public async UniTask <bool> PreloadAsync(RectTransform parent)
		{
			_presenter = _presenterFactory.Create<StatusTabItemPresenter>(parent);
			
			if (_presenter == null)
			{
				Debug.LogError("Failed to instantiate the StatusTabItemPresenter");
				return false;
			}
			
			if (!_presenter.SetMaterial(AreaMaterial))
				return false;
			
			if (AreaMaterial.IsContainer)
			{
				Debug.LogError($"Area material {AreaMaterial} is a container when trying to preload a status tab item");
				return false;
			}
			
			_presenter.AlignToParent();
			
			var materialId = AreaMaterial.MaterialId;
			
			if (materialId == null)
			{
				Debug.LogError($"Missing material id in area material {AreaMaterial}");
				return false;
			}
			
			var material = _materials.Get<WindowMaterialData>(materialId.Value);
			
			if (material == null)
			{
				Debug.LogError($"No window material with id {materialId.Value} was found");
				return false;
			}

			return await PreloadChildAsync(material);
		}

		public void SetParent(RectTransform parent)
		{
			if (_presenter == null)
			{
				Debug.LogError($"StatusTabItemPresenter is not yet available when trying to set the parent for a tab item with content area material: {AreaMaterial}");
				return;
			}
			
			_presenter.SetParentOnInitialize(parent);
			_presenter.AlignToParent();
		}
		
		public bool Show()
		{
			if (_presenter == null || !_presenter.Show())
				return false;
			
			MessageDispatcher.AddListener(MessageType.PreviewMakeReady, OnPreviewMakeReady);
			_child?.Show();
			return true;
		}
		
		public void PerformAction(string actionAlias, ulong materialId) => _child?.PerformAction(actionAlias, materialId);
		
		public void FindOrCreateState(ulong? parentId)
		{
			if (_presenter == null || _child?.Presenter == null || parentId == null)
				return;
			
			var material = _child.Material;
			
			var attribute = material.GetType().GetCustomAttribute<WindowStateAttribute>();
			
			if (attribute == null)
				return;
			
			var areaId = AreaMaterial.Id;
			var states = material.States;
			
			WindowState state = null;
			
			for (var i = states.Length - 1; i >= 0; i--)
			{
				var s = states[i];
				
				if (s == null)
				{
					Debug.LogWarning($"A null state was detected in the states of material: {material}");
					continue;
				}
				
				if (s.StateContentId != parentId || s.AreaId != areaId) 
					continue;

				state = s;
				break;
			}

			if (state == null)
			{
				state = (WindowState) Activator.CreateInstance(attribute.Type);
			
				state.StateContentId = parentId;
				state.AreaId = areaId;
			}
			
			_child.SetState(state);
		}
		
		public bool Hide()
		{
			MessageDispatcher.RemoveListener(MessageType.PreviewMakeReady, OnPreviewMakeReady);
			_child?.Hide();
			return _presenter != null && _presenter.Hide();
		}

		public void Unload()
		{
			MessageDispatcher.RemoveListener(MessageType.PreviewMakeReady, OnPreviewMakeReady);
			
			if (!_unloadCancellationTokenSource.IsCancellationRequested)
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
			}
			
			if (_child != null)
			{
				_screenStreaming.Remove(AreaMaterial.Id);
				
				_child.Unload();
				_child = null;
			}
			
			if (_presenter != null)
			{
				_presenter.Unload();
				_presenter = null;
			}
		}
		
		private async UniTask<bool> PreloadChildAsync(WindowMaterialData material)
		{
			var child = _childControllerFactory.Create(material);
			
			if (child == null)
				return false;
			
			var content = _presenter.Content;
			
			if (!await child.PreloadAsync(content))
				return false;
			
			_child = child;
			
			_screenStreaming.Add(AreaMaterial.Id, _child.Presenter);
			
			return true;
		}
		
		private void OnPreviewMakeReady(IMessage message)
		{
			if (_unloadCancellationTokenSource == null || _presenter == null || _child == null || _child.Presenter != message.Data)
				return;

			var cancellationToken = _unloadCancellationTokenSource.Token;
			
			_previewProvider.MakePreviewAsync(AreaMaterial, _child.Presenter.RectTransform, cancellationToken).Forget();
			
			CancellableMessageDispatcher.SendMessageAsync(MessageType.PreviewScreenModeMakeReady, 3f, cancellationToken).Forget();
		}
	}
}