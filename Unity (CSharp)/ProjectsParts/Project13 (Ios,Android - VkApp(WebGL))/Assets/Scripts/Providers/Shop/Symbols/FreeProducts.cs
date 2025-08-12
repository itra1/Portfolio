using Game.Common.Attributes.Defines;

namespace Game.Providers.Shop.Symbols
{
	public class FreeProducts : IToggleDefine
	{
		public string Symbol => "SHOP_FREE_SYMBOLS";

		public string Description => "(Editor) Продукты в магазине бесплатно";

		public void AfterDisable()
		{
		}

		public void AfterEnable()
		{
		}
	}
}
