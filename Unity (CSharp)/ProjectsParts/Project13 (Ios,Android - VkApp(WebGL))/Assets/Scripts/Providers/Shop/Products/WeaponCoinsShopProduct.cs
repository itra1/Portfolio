using Game.Game.Elements.Weapons;
using Game.Game.Elements.Weapons.Common;
using Game.Providers.Profile.Handlers;
using Game.Providers.Profile.Save;
using Game.Providers.Saves;
using StringDrop;
using UnityEngine;
using Zenject;

namespace Game.Providers.Shop.Products
{
	public class WeaponCoinsShopProduct : ShopProduct
	{
		[SerializeField][StringDropList(typeof(WeaponType))] private string WeaponType;
		[SerializeField] private int _count = 1;

		private ProfileProviderSave _profileSaveGame;
		private CoinsHandler _coinsHandler;
		private IWeaponSpawner _weaponSpawner;

		public override bool IsAlreadyBuyed => false;
		public override string GroupType => "Weapons";
		public override bool IsBuyReady => _coinsHandler.CurrentValue >= Price;

		[Inject]
		public void Build(ISaveProvider saveGame, CoinsHandler coinsHandler, IWeaponSpawner weaponSpawner)
		{
			_profileSaveGame = saveGame.GetProperty<ProfileProviderSave>();
			_coinsHandler = coinsHandler;
			_weaponSpawner = weaponSpawner;
		}

		protected override void ConfirmProduct()
		{
			_ = _coinsHandler.AddValue(-(float) Price, null);

			_weaponSpawner.AddWeapon(WeaponType, _count);
		}
	}
}
