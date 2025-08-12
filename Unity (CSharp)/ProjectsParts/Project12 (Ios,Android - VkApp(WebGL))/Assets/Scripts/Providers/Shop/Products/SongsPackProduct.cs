using Game.Scripts.Providers.Shop.Base;
using Game.Scripts.Providers.Shop.Settings;
using Game.Scripts.UI.Settings;
using itra.Attributes;

namespace Game.Scripts.Providers.Shop.Products
{
	[PrefabName(ProductType.SongsPack)]
	public class SongsPackProduct : ProductProperty<SongsPackProductProperty>
	{
		public override string Type => ProductType.SongsPack;

		public override ProductButtonColorsUiItem ColorData => _shopSettings.ColorsSettings.ColorItems.Find(x => x.Type == _property.Color);
	}
}
