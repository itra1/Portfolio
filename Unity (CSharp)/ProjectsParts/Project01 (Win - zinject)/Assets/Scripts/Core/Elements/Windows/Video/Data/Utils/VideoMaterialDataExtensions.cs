using Core.Elements.Windows.Video.Data.Base;

namespace Core.Elements.Windows.Video.Data.Utils
{
    public static class VideoMaterialDataExtensions
    {
        private const string BgrKey = "bgr";
        private const string MultipleKey = "multiple";
        private const string XDeltaKey = "xDelta";
        private const string YDeltaKey = "hDelta";
        private const string ScaleKey = "scale";
        private const string StreamValidationRequiredKey = "streamValidationRequired";
        
        private const string FillParentValue = "FillParent";
        private const string FillInParentValue = "FillInParent";
        
        public static bool IsBgr(this VideoMaterialData material)
        {
            var options = material.Options;
            return options != null && options.ContainsKey(BgrKey) && options.GetBool(BgrKey);
        }
        
        public static bool IsMultiple(this VideoMaterialData material)
        {
            var options = material.Options;
            return options != null && options.ContainsKey(MultipleKey) && options.GetBool(MultipleKey);
        }

        public static float? GetSizeDeltaX(this VideoMaterialData material)
        {
            var options = material.Options;
            return options != null && options.ContainsKey(XDeltaKey) ? options.GetFloat(XDeltaKey) : null;
        }
        
        public static float? GetSizeDeltaY(this VideoMaterialData material)
        {
            var options = material.Options;
            return options != null && options.ContainsKey(YDeltaKey) ? options.GetFloat(YDeltaKey) : null;
        }
        
        public static bool IsFillParentRequired(this VideoMaterialData material)
        {
            var options = material.Options;
            return options != null && options.ContainsKey(ScaleKey) && options.GetString(ScaleKey) == FillParentValue;
        }
        
        public static bool IsFillInParentRequired(this VideoMaterialData material)
        {
            var options = material.Options;
            return options != null && options.ContainsKey(ScaleKey) && options.GetString(ScaleKey) == FillInParentValue;
        }
        
        public static bool IsStreamValidationRequired(this VideoMaterialData material)
        {
            var options = material.Options;
            return options != null && options.ContainsKey(StreamValidationRequiredKey) && options.GetBool(StreamValidationRequiredKey);
        }
    }
}