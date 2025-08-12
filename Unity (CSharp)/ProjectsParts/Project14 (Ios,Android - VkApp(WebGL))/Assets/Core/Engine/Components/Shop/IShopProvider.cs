using System.Collections.Generic;

namespace Core.Engine.Components.Shop
{
	public interface IShopProvider
	{
		public List<IShopProduct> Items { get; }
	}
}
