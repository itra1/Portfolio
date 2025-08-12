using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.MouseCursor.Presenter.Components
{
    [DisallowMultipleComponent]
    public abstract class MouseCursorUpdaterBase : MonoBehaviour, IMouseCursorUpdater
    {
        [SerializeField] private int _priority;
        
        public string Name => name;
        public int Priority => _priority;
        
        protected IMouseCursorAccess MouseCursorAccess { get; private set; }
        
        protected virtual void Awake() => 
            MouseCursorAccess = ProjectContext.Instance.Container.Resolve<IMouseCursorAccess>();
        
        protected virtual void OnDisable() => 
            MouseCursorAccess.Remove(this);
		
        protected bool IsMouseCursorHover(PointerEventData eventData)
        {
            var hovered = eventData.hovered;
            var count = hovered.Count;
			
            for (var i = 0; i < count; i++)
            {
                if (gameObject != hovered[i])
                    continue;
				
                return true;
            }
			
            return false;
        }
    }
}