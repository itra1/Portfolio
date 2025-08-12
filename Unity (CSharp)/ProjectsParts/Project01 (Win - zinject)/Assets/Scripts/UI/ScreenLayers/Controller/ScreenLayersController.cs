using System;
using System.Threading;
using com.ootii.Messages;
using Core.Materials.Data;
using Core.Messages;
using Core.Options.Offsets;
using Preview;
using UI.Canvas.Presenter;
using UI.ScreenLayers.Presenter;

namespace UI.ScreenLayers.Controller
{
    public class ScreenLayersController : IScreenLayersController, IDisposable
    {
        private IScreenLayersPresenter _presenter;
        private IPreviewProvider _previewProvider;
        private CancellationTokenSource _disposeCancellationTokenSource;
		
        public ScreenLayersController(ICanvasPresenter root, IScreenOffsets screenOffsets, IPreviewProvider previewProvider)
        {
            _presenter = root.ScreenLayers;
            _presenter.Initialize(screenOffsets);
            
            _previewProvider = previewProvider;

            _disposeCancellationTokenSource = new CancellationTokenSource();
            
            MessageDispatcher.AddListener(MessageType.PreviewScreenLayersMakeReady, OnPreviewMakeReady);
        }
        
        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.PreviewScreenLayersMakeReady, OnPreviewMakeReady);
            
            if (_disposeCancellationTokenSource is { IsCancellationRequested: false })
            {
                _disposeCancellationTokenSource.Cancel();
                _disposeCancellationTokenSource.Dispose();
                _disposeCancellationTokenSource = null;
            }
            
            if (_presenter != null)
            {
                _presenter.Unload();
                _presenter = null;
            }
            
            _previewProvider = null;
        }
        
        private void OnPreviewMakeReady(IMessage message)
        {
            if (_presenter != null)
                _previewProvider.MakePreviewAsync((AreaMaterialData) message.Data, _presenter.RectTransform, _disposeCancellationTokenSource.Token).Forget();
        }
    }
}