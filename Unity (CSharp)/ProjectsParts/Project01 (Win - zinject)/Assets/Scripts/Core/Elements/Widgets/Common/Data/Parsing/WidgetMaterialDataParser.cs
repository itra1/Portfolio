using System.Collections.Generic;
using Core.Elements.Base.Data.Parsing;
using Core.Elements.Widgets.Base.Data;
using Core.Materials.Data;
using Core.Materials.Parsing;
using Leguar.TotalJSON;

namespace Core.Elements.Widgets.Common.Data.Parsing
{
    public class WidgetMaterialDataParser: ElementMaterialDataParserBase, IElementMaterialDataParser
    {
        private const string MaterialKey = "material";
        
        public IList<MaterialData> Parse(string rawData)
        {
            var json = JSON.ParseString(rawData);
            var materialJson = json.GetJSON(MaterialKey);
            
            return new []
            {
                ParseMaterialBasedOn<WidgetMaterialData>(materialJson)
            };
        }
    }
}