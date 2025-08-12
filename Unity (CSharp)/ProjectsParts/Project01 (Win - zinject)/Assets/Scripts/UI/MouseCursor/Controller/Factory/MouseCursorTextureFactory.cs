using System;
using Core.Materials.Data;
using Cysharp.Threading.Tasks;
using FileResources;
using UnityEngine;

namespace UI.MouseCursor.Controller.Factory
{
    public class MouseCursorTextureFactory : IMouseCursorTextureFactory
    {
	    private const string UrlKey = "url";
	    
	    private readonly ITextureProvider _textures;
	    
	    public MouseCursorTextureFactory(ITextureProvider textures) => _textures = textures;
	    
        public async UniTask<Texture2D> CreateAsync(MouseCursorMaterialData material) => 
	        await _textures.RequestAsync(ParseImageUrlFrom(material), material.Name);
        
		private string ParseImageUrlFrom(MouseCursorMaterialData material)
		{
			var rawImageData = material.RawImageData;
			
			if (rawImageData == null)
				throw new NullReferenceException("Raw image data is null when trying to parse image url from mouse cursor material data");
			
			if (!rawImageData.ContainsKey(UrlKey))
				throw new NullReferenceException("No URL found in raw image data when trying to parse image url from mouse cursor material data");
			
			var url = rawImageData.GetString(UrlKey);
			
			if (string.IsNullOrEmpty(url))
				throw new NullReferenceException("URL is null of empty when trying to parse image url from mouse cursor material data");
			
			return url;
		}
    }
}