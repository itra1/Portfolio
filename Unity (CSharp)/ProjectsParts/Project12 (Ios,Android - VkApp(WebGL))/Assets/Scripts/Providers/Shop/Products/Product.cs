using Game.Scripts.Providers.Shop.Settings;
using Game.Scripts.UI.Settings;
using Zenject;

namespace Game.Scripts.Providers.Shop.Products
{
	public abstract class Product : IProduct
	{
		protected ShopSettings _shopSettings;

		public abstract void SetProductProperty(ProductPropertyBase productProperty);

		public virtual bool ReadyShow => true;
		public virtual ProductButtonColorsUiItem ColorData => null;

		public abstract string Type { get; }

		[Inject]
		private void Constructor(ShopSettings shopSettings)
		{
			_shopSettings = shopSettings;
		}

	}
}
