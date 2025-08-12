using System;

namespace Core.Materials.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MaterialDataParserAttribute : Attribute
    {
        public Type Type { get; }
        
        public MaterialDataParserAttribute(Type type)
        {
            Type = type;
        }
    }
}