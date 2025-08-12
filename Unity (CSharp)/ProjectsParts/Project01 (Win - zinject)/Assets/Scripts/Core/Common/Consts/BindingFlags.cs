using System.Reflection;

namespace Core.Common.Consts
{
    public static class MemberBindingFlags
    {
        public const BindingFlags PublicInstanceProperty = 
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;
        
        public const BindingFlags PublicInstanceField = 
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField;
    }
}