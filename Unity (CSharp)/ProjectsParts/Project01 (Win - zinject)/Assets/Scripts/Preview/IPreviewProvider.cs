using System.Threading;
using Core.Materials.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Preview
{
    public interface IPreviewProvider : IPreviewState
    {
        UniTaskVoid MakePreviewAsync(AreaMaterialData areaMaterial,
            RectTransform rectTransform,
            CancellationToken cancellationToken);
    }
}