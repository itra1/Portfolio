using System.Collections.Generic;
using Core.Elements.Base.Data.Parsing;
using Core.Elements.StatusColumn.Data;
using Core.Elements.Windows.Base.Data;
using Core.Materials.Data;
using Core.Materials.Parsing;
using Leguar.TotalJSON;

namespace Core.Elements.Status.Data.Parsing
{
    public class StatusMaterialDataParser : ElementMaterialDataParserBase, IElementMaterialDataParser
    {
        private const string StatusKey = "status";
        private const string StatusAreaKey = "statusArea";
        private const string StatusContentAreaKey = "statusContentArea";
        private const string StatusContentKey = "statusContent";
        private const string MaterialKey = "material";

        public IList<MaterialData> Parse(string rawData)
        {
            var materials = new List<MaterialData>();
            
            var json = JSON.ParseString(rawData);
            var statusJson = json.GetJSON(StatusKey);
            var statusAreaJson = json.GetJSON(StatusAreaKey);
            var statusContentAreaJArray = json.GetJArray(StatusContentAreaKey);
            var statusContentJArray = json.GetJArray(StatusContentKey);
            var materialJArray = json.GetJArray(MaterialKey);
            
            materials.Add(ParseMaterial<StatusMaterialData>(statusJson));
            materials.Add(ParseMaterial<StatusAreaMaterialData>(statusAreaJson));
            materials.AddRange(ParseMaterial<StatusContentAreaMaterialData>(statusContentAreaJArray));
            materials.AddRange(ParseMaterial<ContentAreaMaterialData>(statusContentJArray));
            materials.AddRange(ParseMaterialBasedOn<WindowMaterialData>(materialJArray));
            
            return materials.ToArray();
        }
    }
}