using Game.Scripts.Providers.Premiums;
using Game.Scripts.Providers.Shop.Base;
using Game.Scripts.Providers.Shop.Settings;
using itra.Attributes;
using Zenject;

namespace Game.Scripts.Providers.Shop.Products
{
	[PrefabName(ProductType.PremiumSubscribe)]
	public class PremiumSubscribeProduct : ProductProperty<PremiumSubscribeProductProperty>
	{
		private IPremiumProvider _premiumProvider;
		public override string Type => ProductType.PremiumSubscribe;

		[Inject]
		private void Build(IPremiumProvider premiumProvider)
		{
			_premiumProvider = premiumProvider;
		}

		public override bool ReadyShow => !_premiumProvider.IsActive;

	}
}
