using Game.Scripts.Providers.Shop.Base;
using Game.Scripts.Providers.Shop.Settings;
using itra.Attributes;

namespace Game.Scripts.Providers.Shop.Products
{
	[PrefabName(ProductType.WelcomeBundle)]
	public class WelcomeBundleProduct : ProductProperty<WelcomeBundleProductProprty>
	{
		public override string Type => ProductType.WelcomeBundle;
	}
}
