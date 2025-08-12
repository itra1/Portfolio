namespace UI.Audio.Controller.Utils
{
    public static class VolumeConverter
    {
        public const float MinThresholdValue = -80f;
        
        public static float ConvertToThresholdValue(float value) => MinThresholdValue + value * 80f;
        public static float ConvertToValue(float thresholdValue) => (thresholdValue + 80f) * 0.01f;
    }
}