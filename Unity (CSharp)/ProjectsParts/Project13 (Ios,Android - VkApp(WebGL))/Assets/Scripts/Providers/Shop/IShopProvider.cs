using System.Collections.Generic;
using Game.Base.AppLaoder;
using Game.Providers.Shop;

namespace Core.Engine.Components.Shop
{
	public interface IShopProvider : IAppLoaderElement
	{
		public List<IShopProduct> Items { get; }
	}
}
