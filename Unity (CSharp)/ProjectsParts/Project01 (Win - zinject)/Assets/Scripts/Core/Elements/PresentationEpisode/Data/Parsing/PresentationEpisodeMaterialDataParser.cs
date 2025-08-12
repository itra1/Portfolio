using System.Collections.Generic;
using Core.Elements.Base.Data.Parsing;
using Core.Elements.Windows.Base.Data;
using Core.Materials.Data;
using Core.Materials.Parsing;
using Leguar.TotalJSON;

namespace Core.Elements.PresentationEpisode.Data.Parsing
{
    public class PresentationEpisodeMaterialDataParser : ElementMaterialDataParserBase, IElementMaterialDataParser
    {
        private const string EpisodeKey = "episode";
        private const string EpisodeAreaKey = "episodeArea";
        private const string AreaKey = "area";
        private const string MaterialKey = "material";
        
        public IList<MaterialData> Parse(string rawData)
        {
            var materials = new List<MaterialData>();
            
            var json = JSON.ParseString(rawData);
            var episodeJson = json.GetJSON(EpisodeKey);
            var episodeAreaJson = json.GetJSON(EpisodeAreaKey);
            var areaJArray = json.GetJArray(AreaKey);
            var materialJArray = json.GetJArray(MaterialKey);
            
            materials.Add(ParseMaterial<PresentationEpisodeMaterialData>(episodeJson));
            materials.Add(ParseMaterial<PresentationEpisodeAreaMaterialData>(episodeAreaJson));
            materials.AddRange(ParseMaterial<ContentAreaMaterialData>(areaJArray));
            materials.AddRange(ParseMaterialBasedOn<WindowMaterialData>(materialJArray));
            
            return materials.ToArray();
        }
    }
}