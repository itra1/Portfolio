using Core.Elements.Windows.Base.Data;
using Core.Materials.Attributes;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.WebView.Data
{
    public class WebViewState : WindowState
    {
        [MaterialDataPropertyParse("Scale"), MaterialDataPropertyUpdate]
        public float Scale { get; set; }

        [MaterialDataPropertyParse("Url"), MaterialDataPropertyUpdate]
        public string Url { get; set; }
		
        [MaterialDataPropertyParse("ScrollX"), MaterialDataPropertyUpdate]
        public float ScrollX { get; set; }
		
        [MaterialDataPropertyParse("ScrollY"), MaterialDataPropertyUpdate]
        public float ScrollY { get; set; }
        
        public override JSON ConvertToJson()
        {
            var json = base.ConvertToJson();
            
            json.Add("Scale", Scale);
            json.Add("Url", Url);
            json.Add("ScrollX", ScrollX);
            json.Add("ScrollY", ScrollY);
            
            return json;
        }
        
        public override WindowState GetCopyIfInPresentationExcept(ulong areaId,
            ulong? episodeId,
            ulong? presentationId,
            string cloneAlias)
        {
            var copy = (WebViewState) base.GetCopyIfInPresentationExcept(areaId, episodeId, presentationId, cloneAlias);
            
            copy.Scale = Scale;
            copy.Url = Url;
            copy.ScrollX = ScrollX;
            copy.ScrollY = ScrollY;
            
            return copy;
        }
        
        public override WindowState GetCopyIfInStatusExcept(ulong areaId, ulong? statusContentId)
        {
            var copy = (WebViewState) base.GetCopyIfInStatusExcept(areaId, statusContentId);

            copy.Scale = Scale;
            copy.Url = Url;
            copy.ScrollX = ScrollX;
            copy.ScrollY = ScrollY;
            
            return copy;
        }
    }
}