using System;
using System.Linq;

namespace Core.Utils
{
    public static class EnumExtensions 
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute 
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString()).FirstOrDefault();
            
            if (memberInfo == null)
                return null;
            
            var attributes = memberInfo.GetCustomAttributes(typeof(TAttribute), false);
            
            return attributes.Length > 0 
                ? (TAttribute) attributes.First()
                : null;
        }
    }
}