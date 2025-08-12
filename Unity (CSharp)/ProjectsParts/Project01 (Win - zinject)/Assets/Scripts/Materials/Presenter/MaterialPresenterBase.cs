using System;
using System.Threading;
using Base.Presenter;
using com.ootii.Messages;
using Core.Materials.Data;
using Core.Messages;
using Cysharp.Threading.Tasks;
using Preview;
using UnityEngine;
using Debug = Core.Logging.Debug;

namespace Materials.Presenter
{
	public abstract class MaterialPresenterBase<TMaterialData> : PresenterBase, IMaterialPresenter
		where TMaterialData : MaterialData
	{
		[SerializeField] private bool _previewEnabled;
		
		private bool _locked;
		private bool _isWaitingToBeUnlocked;
		private bool _isWaitingToMakePreviewReady;
		private CancellationTokenSource _unloadCancellationTokenSource;
		private IParentArea _parentArea;
		
		public TMaterialData Material { get; private set; }

		protected bool PreviewEnabled => _previewEnabled;
		
		protected CancellationToken UnloadCancellationToken => 
			_unloadCancellationTokenSource?.Token ?? CancellationToken.None;
		
		protected void Initialize(IPreviewState previewState)
		{
			if (_previewEnabled)
				_previewEnabled = previewState.Enabled;
			
			_unloadCancellationTokenSource = new CancellationTokenSource();
		}
		
		public void SetParentArea(IParentArea parentArea) => _parentArea = parentArea;

		public virtual bool SetMaterial(MaterialData material)
		{
			if (material == null)
			{
				Debug.LogError($"An attempt was detected to assign null material to {GetType().Name}");
				return false;
			}

			if (Material != null)
			{
				Debug.LogError($"The material has already been assigned before. Not allowed to reassign material to {GetType().Name}");
				return false;
			}
			
			Material = material as TMaterialData;
			
			if (Material == null)
			{
				Debug.LogError($"Unable to assign material to {GetType().Name} because a material type mismatch is detected. Expected material type is {typeof(TMaterialData).Name}. Assigned material type is {material.GetType()}");
				return false;
			}
			
			var typeName = GetType().Name;
			
			var prefix = typeName.EndsWith(TypeNamePrefix)
				? typeName[..^TypeNamePrefix.Length]
				: typeName;
			
			SetName($"{prefix}: {material.Id} - {material.Name}");
			
			MessageDispatcher.AddListener(Material.UpdateMessageType, OnMaterialDataUpdated);
			
			return true;
		}
		
		public abstract UniTask<bool> PreloadAsync();
		
		public override void Unload()
		{
			if (Material != null)
			{
				MessageDispatcher.RemoveListener(Material.UpdateMessageType, OnMaterialDataUpdated);
				Material = null;
			}
			
			if (_unloadCancellationTokenSource is { IsCancellationRequested: false })
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
			}

			_isWaitingToMakePreviewReady = false;
			_isWaitingToBeUnlocked = false;
			_locked = false;

			_parentArea = null;
			
			base.Unload();
		}
		
		protected bool TryGetParentArea(out IParentArea parentArea)
		{
			if (_parentArea == null)
			{
				parentArea = null;
				return false;
			}
			
			parentArea = _parentArea;
			return true;
		}
		
		protected void Lock() => _locked = true;
		protected void Unlock() => _locked = false;
		
		protected async UniTaskVoid AttemptToNotifyThatReadyToPreviewAsync()
		{
			if (!_previewEnabled || _isWaitingToMakePreviewReady)
				return;
			
			_isWaitingToMakePreviewReady = true;
			
			CancellationTokenSource mergedCancellationTokenSource = null;
			CancellationTokenSource timeoutCancellationTokenSource = null;
			
			try
			{
				var unloadCancellationToken = UnloadCancellationToken;
				
				timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5.0));
				
				mergedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(unloadCancellationToken, 
					timeoutCancellationTokenSource.Token);
				
				if (!Visible)
					await UniTask.WaitUntil(() => Visible, cancellationToken: mergedCancellationTokenSource.Token);
				
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), cancellationToken: unloadCancellationToken);
				
				if (!Visible)
					Debug.LogWarning($"An attempt was detected to create a preview from an invisible presenter named {gameObject.name}");
				else
					MessageDispatcher.SendMessageData(MessageType.PreviewMakeReady, this);
			}
			catch (Exception exception) when (exception is not OperationCanceledException && exception is not ObjectDisposedException)
			{
				Debug.LogException(exception);
			}
			finally
			{
				timeoutCancellationTokenSource?.Dispose();
				mergedCancellationTokenSource?.Dispose();
				
				_isWaitingToMakePreviewReady = false;
			}
		}
		
		protected virtual void HandleMaterialDataUpdate() { }
		
		private async UniTaskVoid ValidateMaterialDataUpdateAsync()
		{
			if (_locked || !Visible)
			{
				if (_isWaitingToBeUnlocked)
					return;
				
				_isWaitingToBeUnlocked = true;
				
				try
				{
					await UniTask.WaitWhile(() => _locked && Visible, cancellationToken: UnloadCancellationToken);
				}
				catch (OperationCanceledException)
				{
					return;
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					return;
				}
				finally
				{
					_isWaitingToBeUnlocked = false;
				}
			}
			
			HandleMaterialDataUpdate();
		}
		
		private void OnMaterialDataUpdated(IMessage message)
		{
			if (Material != null && message.Sender is MaterialData material && material.Id == Material.Id)
				ValidateMaterialDataUpdateAsync().Forget();
		}
	}
}