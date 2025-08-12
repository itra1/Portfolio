using Core.Elements.Windows.Base.Data;
using Core.Materials.Attributes;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.Picture.Data
{
    public class PictureState : WindowState
    {
        [MaterialDataPropertyParse("OffsetX")]
        public float OffsetX { get; set; }
        
        [MaterialDataPropertyParse("OffsetY")]
        public float OffsetY { get; set; }
        
        [MaterialDataPropertyParse("ScaleX")]
        public float ScaleX { get; set; } = 1.0f;
        
        [MaterialDataPropertyParse("ScaleY")] 
        public float ScaleY { get; set; } = 1.0f;
        
        [MaterialDataPropertyParse("ScaleZ")] 
        public float ScaleZ { get; set; } = 1.0f;
        
        public override JSON ConvertToJson()
        {
            var json = base.ConvertToJson();
            
            json.Add("OffsetX", OffsetX);
            json.Add("OffsetY", OffsetY);
            json.Add("ScaleX", ScaleX);
            json.Add("ScaleY", ScaleY);
            json.Add("ScaleZ", ScaleZ);
            
            return json;
        }
        
        public override WindowState GetCopyIfInPresentationExcept(ulong areaId, 
            ulong? episodeId,
            ulong? presentationId,
            string cloneAlias)
        {
            var copy = (PictureState) base.GetCopyIfInPresentationExcept(areaId, episodeId, presentationId, cloneAlias);

            copy.OffsetX = OffsetX;
            copy.OffsetY = OffsetY;
            copy.ScaleX = ScaleX;
            copy.ScaleY = ScaleY;
            copy.ScaleZ = ScaleZ;
            
            return copy;
        }
        
        public override WindowState GetCopyIfInStatusExcept(ulong areaId, ulong? statusContentId)
        {
            var copy = (PictureState) base.GetCopyIfInStatusExcept(areaId, statusContentId);

            copy.OffsetX = OffsetX;
            copy.OffsetY = OffsetY;
            copy.ScaleX = ScaleX;
            copy.ScaleY = ScaleY;
            copy.ScaleZ = ScaleZ;
            
            return copy;
        }
    }
}