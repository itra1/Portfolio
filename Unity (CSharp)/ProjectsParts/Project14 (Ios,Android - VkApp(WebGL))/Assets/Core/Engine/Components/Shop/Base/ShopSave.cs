using System.Collections.Generic;
using Core.Engine.Components.SaveGame;

namespace Core.Engine.Components.Shop
{
	public class ShopSave :SaveProperty<ShopSaveData>
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
