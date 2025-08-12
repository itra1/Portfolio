using System;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Packets.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PacketNameAttribute : Attribute
    {
        public string Name { get; }
        
        public PacketNameAttribute(string name)
        {
            Name = name;
        }
    }
}