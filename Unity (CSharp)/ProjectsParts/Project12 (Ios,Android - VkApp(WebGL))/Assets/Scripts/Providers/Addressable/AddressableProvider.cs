using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AddressablesManager.Runtime.Data;
using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.Addressable.Settings;
using Game.Scripts.Providers.Networks;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Scripts.Providers.Addressable
{
	public class AddressableProvider : IAddressableProvider
	{
		private readonly IAddressableSettings _addressableSettings;
		private readonly INetworkProvider _networkProvider;

		public AddressableProvider(IAddressableSettings addressableSettings, INetworkProvider networkProvider)
		{
			_addressableSettings = addressableSettings;
			_networkProvider = networkProvider;
		}

		public async UniTask StartAppLoad(IProgress<float> onProgress, CancellationToken cancellationToken)
		{
			_ = await Addressables.InitializeAsync().ToUniTask();

			Addressables.ClearResourceLocators();
			CatalogData catalog = null;
			bool existsCatalog = await _networkProvider.Get($"{_addressableSettings.Server}/{_addressableSettings.CatalogFile}", (response) =>
			{
				catalog = Newtonsoft.Json.JsonConvert.DeserializeObject<CatalogData>(response);
			});

			int count = catalog.Builds.Count;
			float progress = 0;
			float progressItem = 1f / count;

			foreach (var item in catalog.Builds)
			{
				var url = $"{_addressableSettings.Server}/{item.CatalogFile}";
				var exists = await _networkProvider.Get(url);

				if (exists)
					await AddContentCatalogIfNotExists(url);
				progress += progressItem;
				onProgress.Report(progress);
			}

			//foreach (var item in _addressableSettings.Catalogs)
			//{
			//	var exists = await _networkProvider.Get(item);

			//	if (exists)
			//		await AddContentCatalogIfNotExists(item);
			//	progress += progressItem;
			//	onProgress.Report(progress);
			//}
			_ = await Addressables.CheckForCatalogUpdates(true);
		}

		public async UniTask<T> LoadAssetAsync<T>(string assetKey, IProgress<float> progress = null, CancellationToken cancellationToken = default)
		{
			bool existsAsset = false;

			var resourceLocators = Addressables.ResourceLocators;
			foreach (IResourceLocator locator in resourceLocators)
			{
				var resourceLocator = Addressables.GetLocatorInfo(locator.LocatorId);

				if (resourceLocator.Locator.Keys.Contains(assetKey))
					existsAsset = true;
			}

			if (!existsAsset)
				return default;

			try
			{
				var handler = Addressables.LoadAssetAsync<T>(assetKey);
				while (!handler.IsDone)
				{
					progress?.Report(handler.PercentComplete);
					await UniTask.Yield();
				}
				progress?.Report(1);

				return handler.Status == AsyncOperationStatus.Succeeded ? handler.Result : default;

			}
			catch
			{
				progress?.Report(1);
				return default;
			}
		}

		private async UniTask AddContentCatalogIfNotExists(string url)
		{
			IEnumerable<IResourceLocator> resourceLocators = Addressables.ResourceLocators;
			bool exists = false;
			foreach (IResourceLocator locator in resourceLocators)
			{
				ResourceLocatorInfo locatorInfo = Addressables.GetLocatorInfo(locator.LocatorId);
				if (locatorInfo != null && locatorInfo.CatalogLocation != null && locator.LocatorId == url)
				{
					exists = true;
				}
			}

			if (!exists)
			{
				_ = await Addressables.LoadContentCatalogAsync(url, true);
			}
		}
	}
}
