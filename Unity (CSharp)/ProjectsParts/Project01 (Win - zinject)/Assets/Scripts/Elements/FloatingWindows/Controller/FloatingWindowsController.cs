using System;
using System.Collections.Generic;
using System.Threading;
using com.ootii.Messages;
using Core.Elements.Windows.Base.Data;
using Core.Materials.Loading.Loader;
using Core.Materials.Loading.Loader.Info;
using Core.Materials.Storage;
using Core.Messages;
using Core.Network.Socket.Packets.Incoming.Actions;
using Core.Network.Socket.Packets.Incoming.Actions.Consts;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.FloatingWindow.Controller;
using Elements.FloatingWindow.Controller.Factory;
using Elements.FloatingWindows.Controller.Key;
using Elements.FloatingWindows.Presenter;
using UnityEngine;

namespace Elements.FloatingWindows.Controller
{
	public class FloatingWindowsController : IFloatingWindowsController, IDisposable
	{
		private readonly IMaterialDataStorage _materials;
		private readonly IMaterialDataLoader _materialLoader;
		private readonly IDictionary<FloatingWindowKey, IFloatingWindowController> _children;
		private readonly IFloatingWindowControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly CancellationTokenSource _unloadCancellationTokenSource;
		
		private IFloatingWindowsPresenter _presenter;
		
		public FloatingWindowsController(IMaterialDataStorage materials,
			IMaterialDataLoader materialLoader,
			IFloatingWindowControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory)
		{
			_materials = materials;
			_materialLoader = materialLoader;
			_children = new Dictionary<FloatingWindowKey, IFloatingWindowController>();
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_unloadCancellationTokenSource = new CancellationTokenSource();
			
			MessageDispatcher.AddListener(MessageType.FloatingWindowOpenByTag, OnFloatingWindowOpenByTag);
			MessageDispatcher.AddListener(MessageType.FloatingWindowMaterialOpen, OnFloatingWindowMaterialOpen);
			MessageDispatcher.AddListener(MessageType.FloatingWindowVideoStreamControl, OnFloatingWindowVideoStreamControl);
			MessageDispatcher.AddListener(MessageType.WindowMaterialAction, OnWindowMaterialAction);
		}

		public bool Preload(RectTransform parent)
		{
			_presenter = _presenterFactory.Create<FloatingWindowsPresenter>(parent);
			
			if (_presenter == null)
			{
				Debug.LogError("Failed to instantiate the FloatingWindowsPresenter");
				return false;
			}
			
			_presenter.AlignToParent();
			_presenter.Show();
			
			return true;
		}
		
		public void Dispose()
		{
			MessageDispatcher.RemoveListener(MessageType.FloatingWindowOpenByTag, OnFloatingWindowOpenByTag);
			MessageDispatcher.RemoveListener(MessageType.FloatingWindowMaterialOpen, OnFloatingWindowMaterialOpen);
			MessageDispatcher.RemoveListener(MessageType.FloatingWindowVideoStreamControl, OnFloatingWindowVideoStreamControl);
			MessageDispatcher.RemoveListener(MessageType.WindowMaterialAction, OnWindowMaterialAction);
			
			if (!_unloadCancellationTokenSource.IsCancellationRequested)
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
			}
			
			foreach (var child in _children.Values)
				child.Unload();
			
			_children.Clear();
			
			if (_presenter != null)
			{
				_presenter.Unload();
				_presenter = null;
			}
		}
		
		private async UniTask<WindowMaterialData> TryLoadWindowMaterialAsync(ulong materialId)
		{
			var info = new MaterialDataLoadingInfo(typeof(WindowMaterialData), materialId);
			
			var result = await _materialLoader.LoadAsync(info);
			
			if (_unloadCancellationTokenSource.IsCancellationRequested)
				return null;
			
			if (!result.Success)
			{
				Debug.LogError($"Failed to load window material by id {materialId}");
				return null;
			}
			
			if (!result.TryGetFirstMaterial<WindowMaterialData>(out var material))
			{
				Debug.LogError($"No window material was found in the loaded list of materials by requested material id {materialId}");
				return null;
			}
			
			return material;
		}
		
		private async UniTask<IFloatingWindowController> PreloadChildAsync(ulong materialId, bool isAdaptiveSizeRequired)
		{
			if (_presenter == null)
				return null;
			
			var material = _materials.Get<WindowMaterialData>(materialId) ?? await TryLoadWindowMaterialAsync(materialId);
			
			if (material == null)
				return null;
			
			var child = _childControllerFactory.Create(material, isAdaptiveSizeRequired);
			
			if (await child.PreloadAsync(_presenter.Content)) 
				return child;
			
			child.Unload();
			return null;
		}
		
		private async UniTask HandleMessageDataAsync(FloatingWindowKey key,
			string tag,
			ulong materialId,
			bool isAllowedToHide,
			bool isAdaptiveSizeRequired)
		{
			if (!_children.TryGetValue(key, out var child))
			{
				_children.Add(key, null);
				
				child = await PreloadChildAsync(materialId, isAdaptiveSizeRequired);
				
				if (child == null)
					return;
				
				_children[key] = child;
			}
			else if (child == null)
			{
				return;
			}
			
			var options = child.Options;
			
			if (options == null)
				return;
			
			options.SetOptions(tag, "SIDEBAR");
			
			if (!child.Active)
				child.Activate();
			else if (isAllowedToHide)
				child.Deactivate();
			
			if (_presenter is { Visible: true } && child.Active)
			{
				if (!child.Visible)
					child.Show();
			}
			else if (child.Visible)
			{
				child.Hide();
			}
		}
		
		private async UniTask HandleMessageDataAsync(FloatingWindowKey key,
			string tag,
			ulong materialId,
			string actionAlias,
			bool isAllowedToHide,
			bool isAdaptiveSizeRequired)
		{
			await HandleMessageDataAsync(key, tag, materialId, isAllowedToHide, isAdaptiveSizeRequired);
			
			if (!string.IsNullOrEmpty(actionAlias))
				PerformFloatingWindowAction(actionAlias, materialId);
		}
		
		private void PerformAction(string actionAlias, ulong materialId)
		{
			foreach (var (key, child) in _children)
			{
				if (key.MaterialId == materialId)
					child.PerformAction(actionAlias, materialId);
			}
		}
		
		private void PerformFloatingWindowAction(string actionAlias, ulong materialId)
		{
			foreach (var (key, child) in _children)
			{
				if (key.MaterialId == materialId)
					child.PerformFloatingWindowAction(actionAlias);
			}
		}
		
		private void OnFloatingWindowOpenByTag(IMessage message)
		{
			var data = (FloatingWindowByTagOpen) message.Data;
			var tag = data.Tag;
			var materialId = data.MaterialId;
			var key = FloatingWindowKey.Get(tag, materialId);
			
			HandleMessageDataAsync(key, tag, materialId, true, true).Forget();
		}
		
		private void OnFloatingWindowMaterialOpen(IMessage message)
		{
			var data = (FloatingWindowMaterialOpen) message.Data;
			var tag = data.Tag;
			var materialId = data.MaterialId;
			var key = FloatingWindowKey.Get(tag, materialId);
			
			HandleMessageDataAsync(key, tag, materialId, true, true).Forget();
		}
		
		private void OnFloatingWindowVideoStreamControl(IMessage message)
		{
			var data = (FloatingWindowVideoStreamPosition) message.Data;
			var tag = data.Tag;
			var materialId = data.MaterialId;
			var actionAlias = data.Alias;
			var key = FloatingWindowKey.Get(tag, materialId);
			
			if ((actionAlias != FloatingWindowVideoStreamMaterialActionAlias.AspectRation && actionAlias != FloatingWindowVideoStreamMaterialActionAlias.PositionReset) 
			    || (_children.TryGetValue(key, out var child) && child.Visible))
				HandleMessageDataAsync(key, tag, materialId, actionAlias, false, false).Forget();
		}
		
		private void OnWindowMaterialAction(IMessage message)
		{
			var action = (WindowMaterialAction) message.Data;
			
			if (action.IsFloatWindow)
				PerformAction(action.Alias, action.MaterialId);
		}
	}
}
