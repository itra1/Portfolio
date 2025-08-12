using System.Collections.Generic;
using Core.Elements.Base.Data.Parsing;
using Core.Elements.Windows.Base.Data;
using Core.Materials.Data;
using Core.Materials.Parsing;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.Common.Data.Parsing
{
    public class WindowMaterialDataParser : ElementMaterialDataParserBase, IElementMaterialDataParser
    {
        public IList<MaterialData> Parse(string rawData)
        {
            var json = JSON.ParseString(rawData);
            
            return new []
            {
                ParseMaterialBasedOn<WindowMaterialData>(json)
            };
        }
    }
}