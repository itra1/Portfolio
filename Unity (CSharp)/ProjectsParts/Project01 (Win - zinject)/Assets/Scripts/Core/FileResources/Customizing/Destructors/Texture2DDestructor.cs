using Core.FileResources.Customizing.Destructors.Base;
using UnityEngine;

namespace Core.FileResources.Customizing.Destructors
{
    public class Texture2DDestructor : IResourceDestructor<Texture2D>
    {
        public void Destruct(Texture2D texture) => Object.Destroy(texture);
    }
}