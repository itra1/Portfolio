using UnityEngine;

namespace FileResources.Info
{
    public class TextureRequestResult : ITextureRequestResult
    {
        public bool InProgress { get; set; }
        public Texture2D Target { get; set; }
        public int OwnerCount { get; set; }
    }
}