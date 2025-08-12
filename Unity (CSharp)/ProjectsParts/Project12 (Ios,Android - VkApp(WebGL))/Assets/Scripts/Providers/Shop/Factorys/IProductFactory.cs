using System.Collections.Generic;
using Game.Scripts.Providers.Shop.Products;

namespace Game.Scripts.Providers.Shop.Factorys
{
	public interface IProductFactory
	{
		List<IProduct> GetProductList();
	}
}