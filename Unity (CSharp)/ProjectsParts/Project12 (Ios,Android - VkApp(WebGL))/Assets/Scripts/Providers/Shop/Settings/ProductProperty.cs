using UnityEngine;

namespace Game.Scripts.Providers.Shop.Settings
{
	public abstract class ProductProperty<IPrice> : ProductPropertyBase
	{
		[SerializeField] private IPrice _price;

		public IPrice Price => _price;
	}
}
