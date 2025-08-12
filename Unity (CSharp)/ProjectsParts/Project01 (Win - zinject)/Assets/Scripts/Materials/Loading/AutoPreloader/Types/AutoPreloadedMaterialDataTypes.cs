using System;
using System.Collections;
using System.Collections.Generic;
using Core.Elements.Windows.Camera.Data;
using Core.Materials.Data;
using Core.Materials.Loading.AutoPreloader.Types;

namespace Materials.Loading.AutoPreloader.Types
{
    public class AutoPreloadedMaterialDataTypes : IAutoPreloadedMaterialDataTypes
    {
        private readonly ISet<Type> _types = new HashSet<Type>
        {
            typeof(UserProfileMaterialData),
            typeof(CameraMaterialData),
            typeof(AudioMaterialData),
            typeof(MouseCursorMaterialData)
        };
        
        public IEnumerator<Type> GetEnumerator() => _types.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}