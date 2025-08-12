using System;

namespace Core.Elements.Windows.Base.Data.Attributes
{
    public class WindowStateAttribute : Attribute
    {
        public Type Type { get; }

        public WindowStateAttribute(Type type)
        {
            Type = type;
        }
    }
}