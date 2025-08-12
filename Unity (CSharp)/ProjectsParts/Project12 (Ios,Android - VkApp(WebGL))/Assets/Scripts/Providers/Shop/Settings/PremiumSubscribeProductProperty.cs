using Game.Scripts.Providers.Shop.Base;
using Game.Scripts.Providers.Shop.Prices;
using UnityEngine;

namespace Game.Scripts.Providers.Shop.Settings
{
	[CreateAssetMenu(fileName = "PremiumSubscribe", menuName = "Providers/Shop/Products/PremiumSubscribe")]
	public class PremiumSubscribeProductProperty : ProductProperty<RealPrice>
	{
		[SerializeField] private int _songCount;
		public override string Type => ProductType.PremiumSubscribe;

		public int SongCount => _songCount;
	}
}
