using System.Collections.Generic;
using Game.Scripts.Providers.Shop.Products;
using Game.Scripts.App;

namespace Game.Scripts.Providers.Shop
{
	public interface IShopProvider : IApplicationLoaderItem
	{
		List<IProduct> ProductList { get; }
	}
}