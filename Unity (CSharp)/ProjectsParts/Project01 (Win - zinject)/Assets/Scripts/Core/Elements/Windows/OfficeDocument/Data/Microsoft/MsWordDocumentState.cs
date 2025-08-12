using Core.Elements.Windows.Base.Data;
using Core.Materials.Attributes;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.OfficeDocument.Data.Microsoft
{
    public class MsWordDocumentState : WindowState
    {
        [MaterialDataPropertyParse("Scroll")]
        public int Scroll { get; set; } = 1; //Starts at value 1, not 0
        
        [MaterialDataPropertyParse("Zoom")]
        public int Zoom { get; set; } = 100; //Range: from 10 to 400
        
        public override JSON ConvertToJson()
        {
            var json = base.ConvertToJson();
            
            json.Add("Scroll", Scroll);
            json.Add("Zoom", Zoom);
            
            return json;
        }
    }
}