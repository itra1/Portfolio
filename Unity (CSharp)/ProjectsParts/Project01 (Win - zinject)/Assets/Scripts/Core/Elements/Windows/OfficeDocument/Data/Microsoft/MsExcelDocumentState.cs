using Core.Elements.Windows.Base.Data;
using Core.Materials.Attributes;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.OfficeDocument.Data.Microsoft
{
    public class MsExcelDocumentState : WindowState
    {
        [MaterialDataPropertyParse("PagePositionX")]
        public int PagePositionX { get; set; } = 1; //Starts at value 1, not 0

        [MaterialDataPropertyParse("PagePositionY")]
        public int PagePositionY { get; set; } = 1; //Starts at value 1, not 0

        [MaterialDataPropertyParse("PageIndex")]
        public int PageIndex { get; set; } = 1; //Starts at value 1, not 0

        [MaterialDataPropertyParse("Zoom")] 
        public int Zoom { get; set; } = 100; //Range: from 10 to 400
        
        public override JSON ConvertToJson()
        {
            var json = base.ConvertToJson();
            
            json.Add("PagePositionX", PagePositionX);
            json.Add("PagePositionY", PagePositionY);
            json.Add("PageIndex", PageIndex);
            json.Add("Zoom", Zoom);
            
            return json;
        }
    }
}