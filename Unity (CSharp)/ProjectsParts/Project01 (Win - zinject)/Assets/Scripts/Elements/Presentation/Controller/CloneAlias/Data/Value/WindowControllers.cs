using System.Collections.Generic;
using Elements.Presentation.Controller.CloneAlias.Data.Key;
using Elements.PresentationEpisodeScreen.Controller;
using Elements.Windows.Common.Controller;

namespace Elements.Presentation.Controller.CloneAlias.Data.Value
{
    public class WindowControllers : IWindowControllers
    {
        private readonly IDictionary<IWindowController, IPresentationEpisodeScreenInfo> _parentsByController;
        
        public WindowAliasKey Key { get; }
        public int Count => _parentsByController.Count;
        public IWindowController Owner { get; private set; }
        
        public WindowControllers(in WindowAliasKey key)
        {
            Key = key;
            _parentsByController = new Dictionary<IWindowController, IPresentationEpisodeScreenInfo>();
        }
        
        public bool IsEmpty() => Count == 0;
        
        public bool TryGetParent(IWindowController controller, out IPresentationEpisodeScreenInfo parent) =>
            _parentsByController.TryGetValue(controller, out parent);
        
        public bool TryGetNext(out IWindowController controller, out IPresentationEpisodeScreenInfo parent)
        {
            controller = null;
            parent = null;
            
            if (_parentsByController.Count == 0)
                return false;
            
            foreach (var (c, p) in _parentsByController)
            {
                if (c == Owner)
                    continue;
                
                controller = c;
                parent = p;
                break;
            }
            
            return controller != null;
        }
        
        public bool TryAdd(IWindowController controller, IPresentationEpisodeScreenInfo parent)
        {
            if (!_parentsByController.TryAdd(controller, parent))
                return false;
            
            controller.PresenterAdded += OnPresenterAdded;
            controller.PresenterRemoved += OnPresenterRemoved;
            
            if (controller.Presenter != null)
                Owner = controller;
            
            return true;
        }

        public bool TryRemove(IWindowController controller)
        {
            if (!_parentsByController.Remove(controller))
                return false;
            
            controller.PresenterAdded -= OnPresenterAdded;
            controller.PresenterRemoved -= OnPresenterRemoved;
            
            if (Owner == controller)
                Owner = null;
            
            return true;
        }
        
        public void Dispose()
        {
            foreach (var controller in _parentsByController.Keys)
            {
                controller.PresenterAdded -= OnPresenterAdded;
                controller.PresenterRemoved -= OnPresenterRemoved;
            }
            
            _parentsByController.Clear();
            
            Owner = null;
        }
        
        private void OnPresenterAdded(IWindowController controller)
        {
            if (Owner != controller)
                Owner = controller;
        }
        
        private void OnPresenterRemoved(IWindowController controller)
        {
            if (Owner == controller) 
                Owner = null;
        }
    }
}