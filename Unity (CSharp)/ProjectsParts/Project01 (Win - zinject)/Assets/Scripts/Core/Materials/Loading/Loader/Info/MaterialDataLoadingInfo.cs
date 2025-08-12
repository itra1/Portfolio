using System;

namespace Core.Materials.Loading.Loader.Info
{
    public readonly struct MaterialDataLoadingInfo
    {
        public Type Type { get; }
        public ulong Id { get; }
        public string UrlPostfix { get; }
        
        public MaterialDataLoadingInfo(Type type, ulong id, string urlPostfix)
        {
            Type = type;
            Id = id;
            UrlPostfix = urlPostfix;
        }
        
        public MaterialDataLoadingInfo(Type type, ulong id)
        {
            Type = type;
            Id = id;
            UrlPostfix = string.Empty;
        }
        
        public bool Equals(MaterialDataLoadingInfo info)
        {
            return Type == info.Type && Id == info.Id;
        }
        
        public override string ToString()
        {
            return $"{{name: \"{Type.Name}\", Id: \"{Id}\"}}";
        }
    }
}