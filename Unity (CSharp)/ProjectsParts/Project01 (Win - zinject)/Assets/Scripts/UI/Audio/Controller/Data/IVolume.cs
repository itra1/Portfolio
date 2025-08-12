namespace UI.Audio.Controller.Data
{
    public interface IVolume
    {
        public float Value { get; set; }
        public bool IsMuted { get; set; }
    }
}