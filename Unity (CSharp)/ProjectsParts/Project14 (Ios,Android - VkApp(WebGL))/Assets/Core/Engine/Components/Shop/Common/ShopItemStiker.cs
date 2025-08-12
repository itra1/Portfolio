using Core.Engine.Components.Skins;
using Zenject;

namespace Core.Engine.Components.Shop
{
	public class ShopItemStiker : ShopProduct
	{
		public string UUIDSkin;

		private ISkinProvider _skinProvider;

		public override string GroupType => ShopProductGroupType.ItemStiker;
		public override bool IsAlreadyBuyed => _skinProvider.IsReadyToSelect(UUIDSkin);


		[Inject]
		public void InitializeProduct(ISkinProvider skinProvider)
		{
			_skinProvider = skinProvider;
		}

		protected override void ConfirmProduct()
		{
			_skinProvider.AddReadySkin(UUIDSkin);
		}
	}
}
