using UnityEngine;

namespace Core.Engine.Components.Skins
{
	public abstract class ItemSkin : Skin
	{
		[SerializeField] protected Texture2D _texture;
		[SerializeField] protected Color _multiplyTexureColor = new(0, 0, 0, 0);

		public override string Type => SkinType.Sticker;
		public Texture2D Texture => _texture;
		public Color MultiplyTexureColor { get => _multiplyTexureColor; set => _multiplyTexureColor = value; }

	}
}
