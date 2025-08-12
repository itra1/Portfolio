using Cysharp.Threading.Tasks;
using UnityEngine;

namespace FileResources
{
    public interface ITextureProvider
    {
        UniTask<Texture2D> RequestAsync(string url, string name);
        void Release(Texture texture);
    }
}