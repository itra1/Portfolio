using Core.Materials.Data;

namespace UI.MusicPlayer.Controller.Converter.Data
{
    public class AudioClipInfo
    {
        public AudioMaterialData Material { get; }
        public int SampleCount { get; }
        public int ChannelCount { get; }
        public int Frequency { get; }
        public float[] Data { get; }
        
        public AudioClipInfo(AudioMaterialData material, int sampleCount, int channelCount, int frequency, float[] data)
        {
            Material = material;
            SampleCount = sampleCount;
            ChannelCount = channelCount;
            Frequency = frequency;
            Data = data;
        }
    }
}