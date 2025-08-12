using Core.Materials.Data;

namespace UI.MusicPlayer.Controller.Converter.Data
{
    public struct AudioTrackInfo
    {
        public AudioMaterialData Material { get; }
        public byte[] Bytes { get; }
        
        public AudioTrackInfo(AudioMaterialData material, byte[] bytes)
        {
            Material = material;
            Bytes = bytes;
        }
    }
}