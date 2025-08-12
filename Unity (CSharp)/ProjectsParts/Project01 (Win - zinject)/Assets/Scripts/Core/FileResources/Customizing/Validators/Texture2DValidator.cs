using Core.FileResources.Customizing.Validators.Base;
using UnityEngine;

namespace Core.FileResources.Customizing.Validators
{
	public class Texture2DValidator : IResourceValidator<Texture2D>
	{
		public bool IsValid(Texture2D texture) => texture.width > 15;
	}
}