using Core.Engine.Components.Skins;
using Zenject;

namespace Core.Engine.Components.Shop.Common
{
	public class ShopItemPlayerMesh : ShopProduct
	{
		public string UUIDMesh;

		private ISkinProvider _skinProvider;

		public override string GroupType => ShopProductGroupType.PlayerMesh;
		public override bool IsAlreadyBuyed => _skinProvider.IsReadyToSelect(UUIDMesh);

		[Inject]
		public void InitializeProduct(ISkinProvider skinProvider)
		{
			_skinProvider = skinProvider;
		}

		protected override void ConfirmProduct()
		{
			_skinProvider.AddReadySkin(UUIDMesh);
		}
	}
}
