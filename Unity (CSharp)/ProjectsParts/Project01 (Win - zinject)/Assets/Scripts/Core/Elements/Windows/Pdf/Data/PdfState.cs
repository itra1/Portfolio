using Core.Elements.Windows.Base.Data;
using Core.Materials.Attributes;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.Pdf.Data
{
    public class PdfState : WindowState
    {
        [MaterialDataPropertyParse("Page"), MaterialDataPropertyUpdate]
        public int Page { get; set; }
        
        [MaterialDataPropertyParse("ZoomFactor"), MaterialDataPropertyUpdate]
        public float ZoomFactor { get; set; }
        
        [MaterialDataPropertyParse("MinZoomFactor"), MaterialDataPropertyUpdate]
        public float MinZoomFactor { get; set; }
        
        [MaterialDataPropertyParse("MaxZoomFactor"), MaterialDataPropertyUpdate]
        public float MaxZoomFactor { get; set; }
        
        public override JSON ConvertToJson()
        {
            var json = base.ConvertToJson();
            
            json.Add("Page", Page);
            json.Add("ZoomFactor", ZoomFactor);
            json.Add("MinZoomFactor", MinZoomFactor);
            json.Add("MaxZoomFactor", MaxZoomFactor);
            
            return json;
        }
        
        public override WindowState GetCopyIfInPresentationExcept(ulong areaId, 
            ulong? episodeId,
            ulong? presentationId,
            string cloneAlias)
        {
            var copy = (PdfState) base.GetCopyIfInPresentationExcept(areaId, episodeId, presentationId, cloneAlias);

            copy.Page = Page;
            copy.ZoomFactor = ZoomFactor;
            copy.MinZoomFactor = MinZoomFactor;
            copy.MaxZoomFactor = MaxZoomFactor;
            
            return copy;
        }
        
        public override WindowState GetCopyIfInStatusExcept(ulong areaId, ulong? statusContentId)
        {
            var copy = (PdfState) base.GetCopyIfInStatusExcept(areaId, statusContentId);

            copy.Page = Page;
            copy.ZoomFactor = ZoomFactor;
            copy.MinZoomFactor = MinZoomFactor;
            copy.MaxZoomFactor = MaxZoomFactor;
            
            return copy;
        }
    }
}