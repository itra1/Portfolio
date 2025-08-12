using System;

namespace Elements.StatusTabItem.Controller.Buffer
{
    public class StatusTabItemDragAndDropBuffer : IStatusTabItemDragAndDropBuffer, IDisposable
    {
        private IStatusTabItemController _tabItem;
        
        public void PutInto(IStatusTabItemController tabItem) => _tabItem = tabItem;
        
        public bool Peek(out IStatusTabItemController tabItem)
        {
            tabItem = _tabItem;
            return tabItem != null;
        }
        
        public void Clear() => _tabItem = null;

        public void Dispose() => Clear();
    }
}