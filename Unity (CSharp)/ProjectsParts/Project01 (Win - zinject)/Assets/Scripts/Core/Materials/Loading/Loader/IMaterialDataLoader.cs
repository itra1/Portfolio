using System;
using System.Collections.Generic;
using Core.Materials.Data;
using Core.Materials.Loading.Loader.Info;
using Cysharp.Threading.Tasks;

namespace Core.Materials.Loading.Loader
{
    public interface IMaterialDataLoader
    {
        bool InProgress { get; }
        UniTask<MaterialDataLoadingResult> LoadAsync(MaterialDataLoadingInfo info);
        void Load(MaterialDataLoadingInfo info, Action<IReadOnlyList<MaterialData>> onCompleted = null, Action<string> onFailure = null);
    }
}