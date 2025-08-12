using System;
using Elements.Common.Presenter.VlcEngine.Utils;
using UnityEngine;

namespace Elements.Common.Presenter.VlcEngine
{
    public class VlcStream : VlcPlayer, IVlcStream
    {
        public event Action<Texture, Material> ImageUpdated;
        public event Action<Texture> SizeAdjusted;
        
        public Texture ImageTexture => Image.texture;
        public Material ImageMaterial => Image.material;
        
        protected override void UpdateImage(Texture texture, Material newMaterial = null)
        {
            ImageUpdated?.Invoke(texture, newMaterial);
            base.UpdateImage(texture, newMaterial);
        }
        
        protected override void AdjustSize(Texture texture)
        {
            this.StretchToFill();
            SizeAdjusted?.Invoke(texture);
        }
    }
}