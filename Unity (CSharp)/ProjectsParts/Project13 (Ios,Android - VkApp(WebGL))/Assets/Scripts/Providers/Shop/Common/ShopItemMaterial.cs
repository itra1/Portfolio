
namespace Game.Providers.Shop
{
	public class ShopItemMaterial : ShopProduct
	{
		public override string GroupType => ShopProductGroupType.ItemMaterial;

		public override bool IsAlreadyBuyed => throw new System.NotImplementedException();

		protected override void ConfirmProduct()
		{
			throw new System.NotImplementedException();
		}
	}
}
