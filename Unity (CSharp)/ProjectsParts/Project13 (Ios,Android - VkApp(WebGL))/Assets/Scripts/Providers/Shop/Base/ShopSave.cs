using System.Collections.Generic;
using Game.Providers.Saves.Data;

namespace Game.Providers.Shop
{
	public class ShopSave : SaveProperty<ShopSaveData>
	{
		public override ShopSaveData DefaultValue => new();
	}

	public class ShopSaveData
	{
		/// <summary>
		/// Купленные продукты
		/// </summary>
		public List<string> BuyedProducts = new();
	}

}
