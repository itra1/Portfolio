using System;

namespace Base
{
    public interface IVisibilityEventDispatcher
    {
        public event Action Shown;
        public event Action Hidden;
    }
}