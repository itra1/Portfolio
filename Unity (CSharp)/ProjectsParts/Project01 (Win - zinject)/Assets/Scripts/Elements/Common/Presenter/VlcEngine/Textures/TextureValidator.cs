using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Elements.Common.Presenter.VlcEngine.Textures
{
    public class TextureValidator : ITextureValidator
    {
        private readonly ITexturesBuffer _texturesBuffer;
        private readonly Random _random;
        
        public TextureValidator(ITexturesBuffer texturesBuffer)
        {
            _texturesBuffer = texturesBuffer;
            _random = new Random();
        }

        public bool Validate(RenderTexture renderTexture)
        {
            try
            {
                var experimentalTexture = _texturesBuffer.ExperimentalTexture;
                
                RandomizePixelsBasedOn(renderTexture, experimentalTexture);
                
                return ValidatePixels(experimentalTexture.GetPixels32());
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        private bool ValidatePixels(IReadOnlyList<Color32> pixels)
        {
            var pixelsCount = pixels.Count;
            
            for (var x = 0; x < pixelsCount - 1; x++)
            {
                var px = pixels[x];
                
                for (var y = x + 1; y < pixelsCount; y++)
                {
                    var py = pixels[y];
                    
                    if (px.a != py.a || px.b != py.b || px.r != py.r || px.g != py.g)
                        return true;
                }
            }
            
            return false;
        }
        
        private void RandomizePixelsBasedOn(RenderTexture baseTexture, Texture2D texture)
        {
            var renderTexture = RenderTexture.active;
            
            RenderTexture.active = baseTexture;
            
            var width = texture.width;
            var height = texture.height;
            
            var maxRandomX = baseTexture.width - 1;
            var maxRandomY = baseTexture.height - 1;
            
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var randomX = _random.Next(maxRandomX);
                    var randomY = _random.Next(maxRandomY);
                    
                    texture.ReadPixels(new Rect(randomX, randomY, 1f, 1f), x, y);
                }
            }
            
            RenderTexture.active = renderTexture;
        }
    }
}