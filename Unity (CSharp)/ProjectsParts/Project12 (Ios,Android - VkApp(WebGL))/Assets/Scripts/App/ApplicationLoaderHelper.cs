using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.Controllers;
using Game.Scripts.Controllers.Sessions;
using Game.Scripts.Providers.Addressable;
using Game.Scripts.Providers.Avatars;
using Game.Scripts.Providers.DailyBonuses;
using Game.Scripts.Providers.DailyMissions;
using Game.Scripts.Providers.Premiums;
using Game.Scripts.Providers.Profiles;
using Game.Scripts.Providers.Saves;
using Game.Scripts.Providers.Shop;
using Game.Scripts.Providers.Songs;
using Game.Scripts.Providers.Songs.Helpers;
using UnityEngine.Events;

namespace Game.Scripts.App
{
	public class ApplicationLoaderHelper : IApplicationLoaderHelper
	{
		public UnityEvent<float> OnProgress { get; set; } = new();

		private readonly List<IApplicationLoaderItem> _loader = new();

		public ApplicationLoaderHelper(
			IApplicationSettingsController appSettingsController,
			ISessionController sessionController,
			ISaveProvider saveProvider,
			ISongsProvider songsProvider,
			IAvatarsProvider avatarProvider,
			IProfileProvider profileProvider,
			IDailyBonusProvider dailyBonusProvider,
			IDailyMissionsProvider dailyMissionsProvider,
			IPremiumProvider premiumProvider,
			IShopProvider shopProvider,
			ISongSaveHelper songSaveHelper,
			IAddressableProvider addressableProvider
		)
		{
			_loader.Add(saveProvider);
			_loader.Add(addressableProvider);
			_loader.Add(appSettingsController);
			_loader.Add(sessionController);
			_loader.Add(avatarProvider);
			_loader.Add(profileProvider);
			_loader.Add(songsProvider);
			_loader.Add(dailyBonusProvider);
			_loader.Add(dailyMissionsProvider);
			_loader.Add(premiumProvider);
			_loader.Add(shopProvider);
			_loader.Add(songSaveHelper);
		}

		public async UniTask AppLoad()
		{
			float progress = 0;
			float progressIncrement = 1f / _loader.Count;

			for (int i = 0; i < _loader.Count; i++)
			{
				var progressStart = progress;

				IProgress<float> progressReporter = new Progress<float>(progressValue =>
				{
					EminProgress(progressStart + (progressValue * progressIncrement));
				});

				await _loader[i].StartAppLoad(progressReporter, default);
				progress = progressStart + progressIncrement;
				EminProgress(progress);
				await UniTask.Yield();
			}
			EminProgress(1);
		}

		private void EminProgress(float progress) => OnProgress?.Invoke(progress);
	}
}
