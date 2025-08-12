using System.Collections.Generic;
using Core.Elements.Base.Data.Parsing;
using Core.Elements.PresentationEpisode.Data;
using Core.Elements.Windows.Base.Data;
using Core.Materials.Data;
using Core.Materials.Parsing;
using Leguar.TotalJSON;

namespace Core.Elements.Presentation.Data.Parsing
{
    public class PresentationMaterialDataParser : ElementMaterialDataParserBase, IElementMaterialDataParser
    {
        private const string PresentationKey = "presentation";
        private const string PresentationAreaKey = "presentationArea";
        private const string EpisodeKey = "episode";
        private const string AreaKey = "area";
        private const string MaterialKey = "material";
        
        public IList<MaterialData> Parse(string rawData)
        {
            var materials = new List<MaterialData>();
            
            var json = JSON.ParseString(rawData);
            var presentationJson = json.GetJSON(PresentationKey);
            var presentationAreaJson = json.GetJSON(PresentationAreaKey);
            var areaJArray = json.GetJArray(AreaKey);
            var episodeJArray = json.GetJArray(EpisodeKey);
            var materialJArray = json.GetJArray(MaterialKey);
            
            materials.Add(ParseMaterial<PresentationMaterialData>(presentationJson));
            materials.Add(ParseMaterial<PresentationAreaMaterialData>(presentationAreaJson));
            materials.AddRange(ParseMaterial<PresentationEpisodeMaterialData>(episodeJArray));
            materials.AddRange(ParseMaterial<PresentationEpisodeAreaMaterialData>(AreaKey, episodeJArray));
            materials.AddRange(ParseMaterial<ContentAreaMaterialData>(areaJArray));
            materials.AddRange(ParseMaterialBasedOn<WindowMaterialData>(materialJArray));

            return materials.ToArray();
        }
    }
}