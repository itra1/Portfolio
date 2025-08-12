using System;
using System.Collections.Generic;
using Core.Engine.Components.Shop;
using Cysharp.Threading.Tasks;
using Game.Game.Elements.Weapons;
using Game.Providers.Audio;
using Game.Providers.Avatars;
using Game.Providers.Battles;
using Game.Providers.DailyBonus;
using Game.Providers.Nicknames;
using Game.Providers.Profile;
using Game.Providers.Saves;
using Game.Providers.Smiles;
using Game.Providers.TimeBonuses;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Base.AppLaoder
{
	public class AppLoaderHandler : IAppLoaderHandler
	{
		public UnityEvent<float> OnProgress { get; set; } = new();

		private List<IAppLoaderElement> _loader = new();

		public AppLoaderHandler(
			ISaveProvider saveProvider,
			INicknamesProvider nicknamesProvider,
			IAvatarsProvider avatarProvider,
			IProfileProvider profileProvider,
			ITimeBonusProvider timeBonusProvider,
			IBattleProvider battleProvider,
			ISmilesProvider smilesProvider,
			IDailyBonusProvider dailyBonusProvider,
			IAudioProvider audioProvider,
			IShopProvider shopProvider,
			IWeaponSpawner weaponSpawner
		)
		{
			Debug.Log($"AppLoaderHandler");
			_loader.Add(saveProvider);
			_loader.Add(audioProvider);
			_loader.Add(nicknamesProvider);
			_loader.Add(avatarProvider);
			_loader.Add(profileProvider);
			_loader.Add(timeBonusProvider);
			_loader.Add(battleProvider);
			_loader.Add(smilesProvider);
			_loader.Add(dailyBonusProvider);
			_loader.Add(shopProvider);
			_loader.Add(weaponSpawner);
		}

		public async UniTask AppLoad()
		{
			float progressIncrement = 1f / _loader.Count;

			for (int i = 0; i < _loader.Count; i++)
			{
				Debug.Log($"load step {i}");
				var progress = progressIncrement * i;
				EminProgress(progress);

				await _loader[i].FirstLoad(new Progress<float>(subprogress =>
				{
					EminProgress(progress + (subprogress * progressIncrement));
				}), default);

				EminProgress(progress);
				await UniTask.Yield();
			}
			EminProgress(1);
		}

		private void EminProgress(float progress) => OnProgress?.Invoke(progress);
	}
}
