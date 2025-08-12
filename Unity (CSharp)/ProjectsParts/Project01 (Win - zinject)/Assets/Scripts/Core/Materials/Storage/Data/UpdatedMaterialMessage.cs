using System.Collections.Generic;
using Core.Materials.Data;

namespace Core.Materials.Storage.Data
{
    public struct UpdatedMaterialMessage
    {
        public MaterialData Material { get; }
        public string MessageType { get; }
        public ISet<string> Parameters { get; }

        public UpdatedMaterialMessage(MaterialData material, string messageType, ISet<string> parameters)
        {
            Material = material;
            MessageType = messageType;
            Parameters = parameters;
        }
    }
}