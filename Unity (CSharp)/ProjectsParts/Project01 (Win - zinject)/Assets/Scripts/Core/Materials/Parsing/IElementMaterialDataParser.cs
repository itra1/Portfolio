using System.Collections.Generic;
using Core.Materials.Data;

namespace Core.Materials.Parsing
{
    public interface IElementMaterialDataParser
    {
        IList<MaterialData> Parse(string rawData);
    }
}