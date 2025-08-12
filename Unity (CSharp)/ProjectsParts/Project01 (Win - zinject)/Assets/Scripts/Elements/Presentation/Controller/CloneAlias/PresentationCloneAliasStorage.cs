using System.Collections.Generic;
using Elements.Presentation.Controller.CloneAlias.Data.Key;
using Elements.Presentation.Controller.CloneAlias.Data.Value;
using Elements.PresentationEpisodeScreen.Controller;
using Elements.Windows.Base.Presenter;
using Elements.Windows.Common.Controller;

namespace Elements.Presentation.Controller.CloneAlias
{
    public class PresentationCloneAliasStorage : IPresentationCloneAliasStorage
    {
        private readonly IDictionary<WindowAliasKey, IWindowControllers> _controllers;
        private readonly IDictionary<IWindowController, WindowAliasKey> _keysByController;
        
        public PresentationCloneAliasStorage()
        {
            _controllers = new Dictionary<WindowAliasKey, IWindowControllers>();
            _keysByController = new Dictionary<IWindowController, WindowAliasKey>();
        }
        
        public int CountBy(IWindowPresenter presenter)
        {
            foreach (var controllers in _controllers.Values)
            {
                if (presenter == controllers.Owner.Presenter)
                    return controllers.Count;
            }
            
            return 0;
        }

        public bool Contains(in WindowAliasKey key)
            => _controllers.ContainsKey(key);
        
        public bool Contains(IWindowController controller)
            => _keysByController.ContainsKey(controller);
        
        public bool ContainsOwner(in WindowAliasKey key)
            => _controllers.TryGetValue(key, out var controllers) && controllers.Owner != null;
        
        public bool TryGetKey(IWindowController controller, out WindowAliasKey key)
            => _keysByController.TryGetValue(controller, out key);
        
        public bool TryGetParent(IWindowController controller, out IPresentationEpisodeScreenInfo parent)
        {
            parent = null;
            
            return _keysByController.TryGetValue(controller, out var key) 
                   && _controllers.TryGetValue(key, out var controllers) 
                   && controllers.TryGetParent(controller, out parent);
        }
        
        public bool TryGetNext(in WindowAliasKey key, out IWindowController controller, out IPresentationEpisodeScreenInfo parent)
        {
            if (_controllers.TryGetValue(key, out var controllers)) 
                return controllers.TryGetNext(out controller, out parent);
            
            controller = null;
            parent = null;
            return false;
        }

        public bool TryGetOwner(in WindowAliasKey key, out IWindowController owner)
        {
            if (!_controllers.TryGetValue(key, out var controllers))
            {
                owner = null;
                return false;
            }
            
            owner = controllers.Owner;
            return owner != null;
        }
        
        public bool Add(IWindowController controller, IPresentationEpisodeScreenInfo parent, in WindowAliasKey key)
        {
            if (!_controllers.TryGetValue(key, out var controllers))
            {
                controllers = new WindowControllers(in key);
                
                if (!controllers.TryAdd(controller, parent))
                {
                    controllers.Dispose();
                    return false;
                }
                
                _controllers.Add(key, controllers);
            }
            else if (!controllers.TryAdd(controller, parent))
            {
                return false;
            }
            
            _keysByController.Add(controller, key);
            return true;
        }
        
        public bool Update(IWindowController controller, IPresentationEpisodeScreenInfo parent, in WindowAliasKey key)
        {
            if (!_keysByController.TryGetValue(controller, out var previousKey))
                return false;
            
            if (!_controllers.TryGetValue(previousKey, out var controllers))
                return false;
            
            controllers.TryRemove(controller);
            
            if (!_controllers.TryGetValue(key, out controllers))
            {
                controllers = new WindowControllers(in key);
                
                if (!controllers.TryAdd(controller, parent))
                {
                    controllers.Dispose();
                    return false;
                }
                
                _controllers.Add(key, controllers);
            }
            else if (!controllers.TryAdd(controller, parent))
            {
                return false;
            }
            
            _keysByController[controller] = key;
            return true;
        }
        
        public bool Remove(IWindowController controller)
        {
            if (!_keysByController.TryGetValue(controller, out var key) || !_keysByController.Remove(controller))
                return false;
            
            if (!_controllers.TryGetValue(key, out var controllers) || !controllers.TryRemove(controller))
                return false;
            
            if (controllers.IsEmpty())
            {
                controllers.Dispose();
                _controllers.Remove(key);
            }
            
            return true;
        }
        
        public void Clear()
        {
            foreach (var controllers in _controllers.Values)
                controllers.Dispose();
            
            _controllers.Clear();
            _keysByController.Clear();
        }
    }
}