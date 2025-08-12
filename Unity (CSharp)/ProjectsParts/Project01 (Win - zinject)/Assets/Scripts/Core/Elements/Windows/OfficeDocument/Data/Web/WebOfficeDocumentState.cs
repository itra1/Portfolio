using Core.Elements.Windows.Base.Data;
using Core.Elements.Windows.WebView.Data;
using Core.Materials.Attributes;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.OfficeDocument.Data.Web
{
    public class WebOfficeDocumentState : WebViewState
    {
        [MaterialDataPropertyParse("Page"), MaterialDataPropertyUpdate]
        public int? Page { get; set; }
        
        public override JSON ConvertToJson()
        {
            var json = base.ConvertToJson();
            json.Add("Page", Page);
            return json;
        }
        
        public override WindowState GetCopyIfInPresentationExcept(ulong areaId,
            ulong? episodeId,
            ulong? presentationId,
            string cloneAlias)
        {
            var copy = (WebOfficeDocumentState) base.GetCopyIfInPresentationExcept(areaId, episodeId, presentationId, cloneAlias);
            copy.Page = Page;
            return copy;
        }
        
        public override WindowState GetCopyIfInStatusExcept(ulong areaId, ulong? statusContentId)
        {
            var copy = (WebOfficeDocumentState) base.GetCopyIfInStatusExcept(areaId, statusContentId);
            copy.Page = Page;
            return copy;
        }
    }
}