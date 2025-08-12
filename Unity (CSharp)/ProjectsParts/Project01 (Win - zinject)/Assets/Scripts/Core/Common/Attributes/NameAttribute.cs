using System;

namespace Core.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NameAttribute : Attribute
    {
        public string Value { get; }

        public NameAttribute(string value)
        {
            Value = value;
        }
    }
}