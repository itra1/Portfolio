using Core.Elements.Windows.Base.Data;
using Core.Materials.Attributes;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.OfficeDocument.Data.Microsoft
{
    public class MsPowerPointDocumentState : WindowState
    {
        [MaterialDataPropertyParse("Slide")] 
        public int Slide { get; set; } = 1;
        
        public override JSON ConvertToJson()
        {
            var json = base.ConvertToJson();
            
            json.Add("Slide", Slide);
            
            return json;
        }
    }
}