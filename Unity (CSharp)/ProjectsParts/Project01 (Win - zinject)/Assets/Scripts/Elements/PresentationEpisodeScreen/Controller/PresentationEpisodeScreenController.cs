using System;
using System.Reflection;
using System.Threading;
using Base;
using Base.Presenter;
using com.ootii.Messages;
using Core.Elements.Windows.Base.Data;
using Core.Elements.Windows.Base.Data.Attributes;
using Core.Materials.Data;
using Core.Materials.Loading.Loader;
using Core.Materials.Loading.Loader.Info;
using Core.Materials.Storage;
using Core.Messages;
using Core.Network.Socket.Packets.Incoming.Actions.Consts;
using Core.Utils;
using Cysharp.Threading.Tasks;
using Elements.Common.Presenter.Factory;
using Elements.Presentation.Controller.CloneAlias;
using Elements.Presentation.Controller.CloneAlias.Data.Key;
using Elements.PresentationEpisodeScreen.Presenter;
using Elements.Windows.Base;
using Elements.Windows.Common.Controller;
using Elements.Windows.Common.Controller.Factory;
using Preview;
using ScreenStreaming;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Elements.PresentationEpisodeScreen.Controller
{
	public class PresentationEpisodeScreenController : IPresentationEpisodeScreenController
	{
		private readonly ulong? _presentationId;
		private readonly IMaterialDataLoader _materialLoader;
		private readonly IMaterialDataStorage _materials;
		private readonly IPresentationCloneAliasStorage _cloneAliasStorage;
		private readonly IWindowControllerFactory _childControllerFactory;
		private readonly IPresenterFactory _presenterFactory;
		private readonly IPreviewProvider _previewProvider;
		private readonly IScreenStreamingController _screenStreaming;
		private readonly CancellationTokenSource _unloadCancellationTokenSource;

		private IWindowController _child;

		public ContentAreaMaterialData AreaMaterial { get; }

		public IPresentationEpisodeScreenPresenter Presenter { get; private set; }

		public PresentationEpisodeScreenController(ulong? presentationId,
			ContentAreaMaterialData areaMaterial,
			IMaterialDataLoader materialLoader,
			IMaterialDataStorage materials,
			IPresentationCloneAliasStorage cloneAliasStorage,
			IWindowControllerFactory childControllerFactory,
			IPresenterFactory presenterFactory,
			IPreviewProvider previewProvider,
			IScreenStreamingController streamingController)
		{
			_presentationId = presentationId;

			AreaMaterial = areaMaterial;

			_materialLoader = materialLoader;
			_materials = materials;
			_cloneAliasStorage = cloneAliasStorage;
			_childControllerFactory = childControllerFactory;
			_presenterFactory = presenterFactory;
			_previewProvider = previewProvider;
			_screenStreaming = streamingController;
			_unloadCancellationTokenSource = new CancellationTokenSource();

			MessageDispatcher.AddListener(AreaMaterial.UpdateMessageType, OnAreaMaterialDataUpdated);
		}

		public async UniTask<bool> PreloadAsync(RectTransform parent)
		{
			Presenter = _presenterFactory.Create<PresentationEpisodeScreenPresenter>(parent);

			if (Presenter == null)
			{
				Debug.LogError($"Failed to instantiate the PresentationEpisodeScreenPresenter by area material {AreaMaterial}");
				return false;
			}

			if (!Presenter.SetMaterial(AreaMaterial))
				return false;

			if (AreaMaterial.IsContainer)
			{
				Debug.LogError($"Area material {AreaMaterial} is a container when trying to preload a presentation episode screen");
				return false;
			}

			Presenter.AlignToParent();

			var materialId = AreaMaterial.MaterialId;

			if (materialId == null)
			{
				Debug.LogError($"Missing material id in area material {AreaMaterial}");
				return false;
			}

			var material = await TryGetWindowMaterialAsync(materialId.Value);

			return material != null && await PreloadChildAsync(material);
		}

		public bool Show()
		{
			if (Presenter == null || !Presenter.Show())
				return false;

			MessageDispatcher.AddListener(MessageType.PreviewMakeReady, OnPreviewMakeReady);

			if (_child != null)
			{
				var cloneAlias = AreaMaterial.CloneAlias;

				if (!string.IsNullOrEmpty(cloneAlias))
				{
					var key = WindowAliasKey.Get(cloneAlias, _presentationId, _child.Material);

					if (_child.Presenter == null)
					{
						if (_cloneAliasStorage.TryGetOwner(in key, out var owner))
							ChangePresenterOwner(owner, _child, in key, this);
						else
							Debug.LogError($"Missing presenter owner by key {key}");
					}
				}

				_ = _child.Show();
			}

			return true;
		}

		public void PerformAction(string actionAlias, ulong materialId)
		{
			if (Presenter == null || _child == null || _child.Material.Id != materialId)
				return;

			if (actionAlias == WindowMaterialActionAlias.Focus)
			{
				if (!Presenter.InFocus)
					Presenter.Focus();
				else
					Presenter.Unfocus();
			}
			else
			{
				_child.PerformAction(actionAlias, materialId);
			}
		}

		public void FindOrCreateState(ulong? parentId)
		{
			if (Presenter == null || _child?.Presenter == null || AreaMaterial.ParentId != parentId)
				return;

			var material = _child.Material;

			var attribute = material.GetType().GetCustomAttribute<WindowStateAttribute>();

			if (attribute == null)
				return;

			var areaId = AreaMaterial.Id;
			var cloneAlias = AreaMaterial.CloneAlias;
			var states = material.States;

			WindowState state = null;

			if (!string.IsNullOrEmpty(cloneAlias))
			{
				for (var i = states.Length - 1; i >= 0; i--)
				{
					var s = states[i];

					if (s == null)
					{
						Debug.LogWarning($"A null state was detected in the states of material: {material}");
						continue;
					}

					if (s.PresentationId != _presentationId || s.CloneAlias != cloneAlias)
						continue;

					state = s;
					break;
				}

				if (state == null)
				{
					state = (WindowState) Activator.CreateInstance(attribute.Type);

					state.EpisodeId = parentId;
					state.AreaId = areaId;
					state.PresentationId = _presentationId;
					state.CloneAlias = cloneAlias;
				}
			}
			else
			{
				for (var i = states.Length - 1; i >= 0; i--)
				{
					var s = states[i];

					if (s == null)
					{
						Debug.LogWarning($"A null state was detected in the states of material: {material}");
						continue;
					}

					if (s.PresentationId != _presentationId || s.EpisodeId != parentId || s.AreaId != areaId)
						continue;

					state = s;
					break;
				}

				if (state == null)
				{
					state = (WindowState) Activator.CreateInstance(attribute.Type);

					state.EpisodeId = parentId;
					state.AreaId = areaId;
					state.PresentationId = _presentationId;
				}
			}

			_ = _child.SetState(state);
		}

		public bool Hide()
		{
			MessageDispatcher.RemoveListener(MessageType.PreviewMakeReady, OnPreviewMakeReady);
			_ = (_child?.Hide());
			return Presenter != null && Presenter.Hide();
		}

		public void Unload()
		{
			MessageDispatcher.RemoveListener(AreaMaterial.UpdateMessageType, OnAreaMaterialDataUpdated);
			MessageDispatcher.RemoveListener(MessageType.PreviewMakeReady, OnPreviewMakeReady);

			if (!_unloadCancellationTokenSource.IsCancellationRequested)
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
			}

			UnloadChild();

			if (Presenter != null)
			{
				Presenter.Unload();
				Presenter = null;
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

		private async UniTask<WindowMaterialData> TryGetWindowMaterialAsync(ulong materialId)
		{
			var material = _materials.Get<WindowMaterialData>(materialId) ?? await TryLoadWindowMaterialAsync(materialId);

			if (material == null)
				Debug.LogError($"No window material with id {materialId} was found");

			return material;
		}

		private async UniTask<bool> PreloadChildAsync(WindowMaterialData material)
		{
			var child = _childControllerFactory.Create(material);

			if (child == null)
				return false;

			var content = Presenter.Content;
			var cloneAlias = AreaMaterial.CloneAlias;

			IRenderStreamingCapable target;

			if (!string.IsNullOrEmpty(cloneAlias))
			{
				var key = WindowAliasKey.Get(cloneAlias, _presentationId, material);

				if (!_cloneAliasStorage.Contains(in key))
				{
					_ = _cloneAliasStorage.Add(child, this, in key);

					if (!await child.PreloadAsync(content))
					{
						_ = _cloneAliasStorage.Remove(child);
						return false;
					}

					target = child.Presenter;
				}
				else
				{
					if (!_cloneAliasStorage.TryGetOwner(in key, out var owner) || owner.Presenter == null)
					{
						try
						{
							await UniTask.WaitUntil(() => !_cloneAliasStorage.Contains(in key) || (_cloneAliasStorage.TryGetOwner(in key, out owner) && owner.Presenter != null),
								cancellationToken: _unloadCancellationTokenSource.Token);
						}
						catch (OperationCanceledException)
						{
							return false;
						}
						catch (Exception exception)
						{
							Debug.LogException(exception);
							return false;
						}
					}

					_ = _cloneAliasStorage.Add(child, this, in key);

					if (!_cloneAliasStorage.ContainsOwner(in key))
					{
						if (!await child.PreloadAsync(content))
						{
							_ = _cloneAliasStorage.Remove(child);
							return false;
						}

						target = child.Presenter;
					}
					else
					{
						target = owner.Presenter;
					}
				}
			}
			else
			{
				if (!await child.PreloadAsync(content))
					return false;

				target = child.Presenter;
			}

			_child = child;

			_screenStreaming.Add(AreaMaterial.Id, target);

			return true;
		}

		private async UniTask<bool> ReloadChildAsync(ulong? materialId)
		{
			var material = materialId != null ? await TryGetWindowMaterialAsync(materialId.Value) : null;

			UnloadChild();

			if (material == null || !await PreloadChildAsync(material) || _child == null)
				return false;

			FindOrCreateState(AreaMaterial.ParentId);
			return true;
		}

		private async UniTask<bool> ValidateChildAsync(WindowState state)
		{
			var content = Presenter.Content;

			if (_child.Presenter == null)
			{
				if (!await _child.PreloadAsync(content))
					return false;

				if (state != null)
				{
					_ = _child.SetState(state.GetCopyIfInPresentationExcept(AreaMaterial.Id,
						AreaMaterial.ParentId,
						_presentationId,
						AreaMaterial.CloneAlias));
				}
			}

			return true;
		}

		private async UniTask<bool> ActualizeChildAsync(bool isRequiredToValidateIfNecessary, WindowState state = null)
		{
			var materialId = AreaMaterial.MaterialId;

			if (materialId != null && _child?.Material != null && materialId.Value == _child.Material.Id)
			{
				if (isRequiredToValidateIfNecessary && !await ValidateChildAsync(state))
					return false;
			}
			else
			{
				if (!await ReloadChildAsync(materialId))
					return false;
			}

			return true;
		}

		private void UnloadChild()
		{
			if (_child == null)
				return;

			if (_cloneAliasStorage.TryGetKey(_child, out var key))
			{
				if (_cloneAliasStorage.TryGetOwner(in key, out var owner)
						&& _child == owner
						&& _cloneAliasStorage.TryGetNext(in key, out var nextOwner, out var parent))
				{
					ChangePresenterOwner(_child, nextOwner, in key, parent);
				}

				_ = _cloneAliasStorage.Remove(_child);
			}

			_screenStreaming.Remove(AreaMaterial.Id);

			if (!_cloneAliasStorage.Contains(in key))
				_child.Unload();

			_child = null;
		}

		private void ChangePresenterOwner(IWindowPresenterOwner currentOwner,
			IWindowController nextOwner,
			in WindowAliasKey key,
			IPresentationEpisodeScreenInfo parent)
		{
			if (nextOwner.TryExtractPresenter(out var presenter) && _cloneAliasStorage.CountBy(presenter) <= 1)
				presenter.Unload();

			nextOwner.SetPresenterForcibly(currentOwner.ExtractPresenter(), parent.Presenter.Content);

			var nextOwnerState = nextOwner.GetState();

			if (nextOwnerState != null)
			{
				nextOwnerState.EpisodeId = parent.AreaMaterial.ParentId;
				nextOwnerState.AreaId = parent.AreaMaterial.Id;
			}
			else
			{
				Debug.LogError($"Failed to get state of next window presenter owner with material {nextOwner.Material} when trying to change owner");
			}

			if (_cloneAliasStorage.Contains(nextOwner))
				_ = _cloneAliasStorage.Update(nextOwner, parent, in key);
			else
				_ = _cloneAliasStorage.Add(nextOwner, parent, in key);
		}

		private async UniTask<bool> HandleAreaMaterialDataUpdateAsync()
		{
			var cloneAlias = AreaMaterial.CloneAlias;

			if (_cloneAliasStorage.TryGetKey(_child, out var key))
			{
				if (!string.IsNullOrEmpty(cloneAlias))
				{
					var newKey = WindowAliasKey.Get(cloneAlias, _presentationId, _child.Material);

					if (key == newKey)
						return false;

					var state = _child.GetState();

					if (_cloneAliasStorage.TryGetOwner(in key, out var currentOwner)
							&& _child == currentOwner
							&& _cloneAliasStorage.TryGetNext(in key, out var nextOwner, out var parent))
					{
						ChangePresenterOwner(_child, nextOwner, in key, parent);
					}

					if (_cloneAliasStorage.TryGetParent(_child, out parent))
					{
						_ = _cloneAliasStorage.Remove(_child);

						if (!_cloneAliasStorage.Contains(in newKey) || !_cloneAliasStorage.TryGetOwner(in newKey, out var previousOwner))
						{
							if (!await ValidateChildAsync(state))
								return false;

							_ = _cloneAliasStorage.Add(_child, parent, in newKey);
						}
						else
						{
							ChangePresenterOwner(previousOwner, _child, in newKey, parent);
						}
					}
					else
					{
						Debug.LogError($"Missing parent of child with material {_child.Material}");
					}
				}
				else
				{
					var state = _child.GetState();

					if (_cloneAliasStorage.TryGetOwner(in key, out var owner)
							&& _child == owner
							&& _cloneAliasStorage.TryGetNext(in key, out var nextOwner, out var parent))
					{
						ChangePresenterOwner(_child, nextOwner, in key, parent);
					}

					_ = _cloneAliasStorage.Remove(_child);

					if (!await ActualizeChildAsync(true, state))
						return false;
				}
			}
			else if (!string.IsNullOrEmpty(cloneAlias))
			{
				var newKey = WindowAliasKey.Get(cloneAlias, _presentationId, _child.Material);

				if (_cloneAliasStorage.TryGetOwner(in newKey, out var owner))
				{
					ChangePresenterOwner(owner, _child, in newKey, this);
				}
				else
				{
					var state = _child.GetState();

					if (!await ActualizeChildAsync(true, state))
						return false;

					_ = _cloneAliasStorage.Add(_child, this, in newKey);
				}
			}
			else
			{
				if (!await ActualizeChildAsync(false))
					return false;
			}

			return true;
		}

		private async UniTaskVoid AttemptToUpdateAsync()
		{
			if (!await HandleAreaMaterialDataUpdateAsync())
				return;

			Presenter.AlignToParent();

			if (Presenter.Visible && _child.Presenter is { Visible: false })
				_ = _child.Show();

			if (_child.Presenter is IAlignable alignableChild)
				alignableChild.AlignToParent();
		}

		private void OnPreviewMakeReady(IMessage message)
		{
			if (_unloadCancellationTokenSource == null || Presenter == null || _child == null || _child.Presenter != message.Data)
				return;

			var cancellationToken = _unloadCancellationTokenSource.Token;

			_previewProvider.MakePreviewAsync(AreaMaterial, _child.Presenter.RectTransform, cancellationToken).Forget();

			CancellableMessageDispatcher.SendMessageAsync(MessageType.PreviewScreenModeMakeReady, 3f, cancellationToken).Forget();
		}

		private void OnAreaMaterialDataUpdated(IMessage message)
		{
			if (Presenter == null || AreaMaterial == null)
				return;

			if (message.Sender is not ContentAreaMaterialData areaMaterial || areaMaterial.Id != AreaMaterial.Id)
				return;

			AttemptToUpdateAsync().Forget();
		}
	}
}