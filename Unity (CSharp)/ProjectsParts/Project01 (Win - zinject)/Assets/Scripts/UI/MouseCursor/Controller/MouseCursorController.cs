using System;
using com.ootii.Messages;
using Core.Materials.Data;
using Core.Materials.Storage;
using Core.Messages;
using Cysharp.Threading.Tasks;
using Settings.Data;
using UI.MouseCursor.Controller.Factory;
using UI.MouseCursor.Presenter;
using Debug = Core.Logging.Debug;

namespace UI.MouseCursor.Controller
{
    public class MouseCursorController : IMouseCursorController, IDisposable
    {
        private readonly IMaterialDataStorage _materials;
        private readonly IMouseCursorTextureFactory _factory;
        private readonly IMouseCursorPresenter _presenter;
        
        private ulong? _selectedMaterialId;
        
        public MouseCursorController(IMaterialDataStorage materials, 
            IMouseCursorTextureFactory factory, 
            IMouseCursorPresenter presenter)
        {
            _materials = materials;
            _factory = factory;
            _presenter = presenter;
            
            MessageDispatcher.AddListener(MessageType.MouseCursorLoad, OnMouseCursorMaterialDataLoaded);
            MessageDispatcher.AddListener(MessageType.MouseCursorSelect, OnMouseCursorSelected);
        }

        public void Dispose()
        {
            MessageDispatcher.RemoveListener(MessageType.MouseCursorLoad, OnMouseCursorMaterialDataLoaded);
            MessageDispatcher.RemoveListener(MessageType.MouseCursorSelect, OnMouseCursorSelected);
            
            _selectedMaterialId = null;
        }
        
        private async UniTaskVoid HandleMouseCursorMaterialAsync(MouseCursorMaterialData material)
        {
            try
            {
                var texture = await _factory.CreateAsync(material);
                
                if (texture == null)
                    throw new NullReferenceException("Mouse cursor state texture has not been created");
                
                var info = new MouseCursorInfo
                {
                    State = MouseCursorState.Focus,
                    Texture = texture,
                    HotSpot = default
                };
                
                _presenter.SetInfo(info);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }
        
        private void OnMouseCursorMaterialDataLoaded(IMessage message)
        {
            if (_selectedMaterialId == null)
                return;
            
            var id = _selectedMaterialId.Value;
            var material = _materials.Get<MouseCursorMaterialData>(id);
            
            if (material != null)
                HandleMouseCursorMaterialAsync(material).Forget();
        }
        
        private void OnMouseCursorSelected(IMessage message)
        {
            var id = (ulong) message.Data;
            
            var material = _materials.Get<MouseCursorMaterialData>(id);
            
            if (material == null)
                _selectedMaterialId = id;
            else
                HandleMouseCursorMaterialAsync(material).Forget();
        }
    }
}