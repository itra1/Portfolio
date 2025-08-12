namespace UI.Audio.Controller.Data
{
    public class Volume : IVolume
    {
        public float Value { get; set; }
        public bool IsMuted { get; set; }
        
        public Volume(float value, bool isMuted = false)
        {
            Value = value;
            IsMuted = isMuted;
        }
    }
}