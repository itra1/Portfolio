using System;
using System.Threading;
using Base.Presenter;
using com.ootii.Messages;
using Core.Elements.Desktop.Data;
using Core.Messages;
using Cysharp.Threading.Tasks;
using Preview;
using Debug = Core.Logging.Debug;

namespace Elements.Desktop.Presenter
{
	public class DesktopPresenter : PresenterBase, IDesktopPresenter
	{
		private CancellationTokenSource _unloadCancellationTokenSource;
		
		private bool _previewEnabled;
		
		private DesktopMaterialData _material;
		private DesktopAreaMaterialData _areaMaterial;
		
		public void Initialize(IPreviewState previewState)
		{
			_unloadCancellationTokenSource = new CancellationTokenSource();
			_previewEnabled = previewState.Enabled;
		}

		public bool SetMaterials(DesktopMaterialData material, DesktopAreaMaterialData areaMaterial)
		{
			if (material == null || areaMaterial == null)
			{
				Debug.LogError("An attempt was detected to assign a null material or a null area material to the DesktopPresenter");
				return false;
			}
			
			if (_material != null || _areaMaterial != null)
			{
				Debug.LogError("Materials have already been assigned before. Not allowed to reassign material or area material in the DesktopPresenter");
				return false;
			}
			
			_material = material;
			_areaMaterial = areaMaterial;
			
			SetName($"Desktop: [{areaMaterial.Id}, {material.Id}] - {material.Name}");
			
			return true;
		}

		public override bool Show()
		{
			if (!base.Show())
				return false;
			
			NotifyThatReadyToPreviewAsync().Forget();
			return true;
		}
		
		public override void Unload()
		{
			if (_unloadCancellationTokenSource is { IsCancellationRequested: false })
			{
				_unloadCancellationTokenSource.Cancel();
				_unloadCancellationTokenSource.Dispose();
			}
			
			base.Unload();
		}
		
		private async UniTaskVoid NotifyThatReadyToPreviewAsync()
		{
			if (!_previewEnabled)
				return;
			
			try
			{
				if (!Visible)
					return;
                
				await UniTask.NextFrame(cancellationToken: _unloadCancellationTokenSource.Token);
                
				if (Visible)
					MessageDispatcher.SendMessageData(MessageType.PreviewMakeReady, this);
			}
			catch (Exception exception) when (exception is not OperationCanceledException)
			{
				Debug.LogException(exception);
			}
		}
	}
}