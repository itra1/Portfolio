using Core.Materials.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.MouseCursor.Controller.Factory
{
    public interface IMouseCursorTextureFactory
    {
        UniTask<Texture2D> CreateAsync(MouseCursorMaterialData material);
    }
}