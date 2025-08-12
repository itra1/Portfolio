using Core.Elements.Windows.Base.Data;
using Core.Materials.Attributes;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.Video.Data
{
    public class VideoState : WindowState
    {
        [MaterialDataPropertyParse("time")]
        public float Time { get; set; }
        
        [MaterialDataPropertyParse("loop")]
        public bool Loop { get; set; }
        
        public override JSON ConvertToJson()
        {
            var json = base.ConvertToJson();
            
            json.Add("time", Time);
            json.Add("loop", Loop);
            
            return json;
        }
        
        public override WindowState GetCopyIfInPresentationExcept(ulong areaId, 
            ulong? episodeId,
            ulong? presentationId,
            string cloneAlias)
        {
            var copy = (VideoState) base.GetCopyIfInPresentationExcept(areaId, episodeId, presentationId, cloneAlias);
            
            copy.Time = Time;
            copy.Loop = Loop;
            
            return copy;
        }
        
        public override WindowState GetCopyIfInStatusExcept(ulong areaId, ulong? statusContentId)
        {
            var copy = (VideoState) base.GetCopyIfInStatusExcept(areaId, statusContentId);

            copy.Time = Time;
            copy.Loop = Loop;
            
            return copy;
        }
    }
}