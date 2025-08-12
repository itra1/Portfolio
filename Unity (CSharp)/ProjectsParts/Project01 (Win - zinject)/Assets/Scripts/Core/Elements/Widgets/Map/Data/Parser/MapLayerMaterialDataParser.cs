using System.Collections.Generic;
using Core.Elements.Base.Data.Parsing;
using Core.Materials.Data;
using Core.Materials.Parsing;
using Leguar.TotalJSON;

namespace Core.Elements.Widgets.Map.Data.Parser
{
    public class MapLayerMaterialDataParser : ElementMaterialDataParserBase, IElementMaterialDataParser
    {
        private const string LocationsKey = "locations";
        
        public IList<MaterialData> Parse(string rawData)
        {
            var materials = new List<MaterialData>();
            
            var json = JSON.ParseString(rawData);
            var locationsJArray = json.GetJArray(LocationsKey);
            
            materials.Add(ParseMaterial<MapLayerMaterialData>(json));
            materials.AddRange(ParseMaterial<MapLocationMaterialData>(locationsJArray));
            
            return materials.ToArray();
        }
    }
}