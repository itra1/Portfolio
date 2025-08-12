using System.Collections.Generic;
using Core.Elements.Base.Data.Parsing;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Data;
using Core.Materials.Parsing;
using Leguar.TotalJSON;

namespace Core.Elements.Desktop.Data.Parsing
{
    public class DesktopMaterialDataParser : ElementMaterialDataParserBase, IElementMaterialDataParser
    {
        private const string DesktopKey = "desktop";
        private const string AreaKey = "area";
        private const string ContentAreaKey = "contentArea";
        private const string MaterialKey = "material";
        
        public IList<MaterialData> Parse(string rawData)
        {
            var materials = new List<MaterialData>();
            
            var json = JSON.ParseString(rawData);
            var desktopJson = json.GetJSON(DesktopKey);
            var areaJson = json.GetJSON(AreaKey);
            var contentAreaJArray = json.GetJArray(ContentAreaKey);
            var materialJArray = json.GetJArray(MaterialKey);
            
            materials.Add(ParseMaterial<DesktopMaterialData>(desktopJson));
            materials.Add(ParseMaterial<DesktopAreaMaterialData>(areaJson));
            materials.AddRange(ParseMaterial<ContentAreaMaterialData>(contentAreaJArray));
            materials.AddRange(ParseMaterial<WidgetMaterialData>(materialJArray));
            
            return materials.ToArray();
        }
    }
}