using UnityEngine;

namespace FileResources.Info
{
    public interface ITextureRequestResult
    {
        bool InProgress { get; set; }
        Texture2D Target { get; set; }
        int OwnerCount { get; set; }
    }
}