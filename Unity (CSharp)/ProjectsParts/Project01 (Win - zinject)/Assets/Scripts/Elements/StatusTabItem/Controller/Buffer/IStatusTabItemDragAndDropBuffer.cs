namespace Elements.StatusTabItem.Controller.Buffer
{
    public interface IStatusTabItemDragAndDropBuffer
    {
        public void PutInto(IStatusTabItemController tabItem);
        public bool Peek(out IStatusTabItemController tabItem);
        public void Clear();
    }
}