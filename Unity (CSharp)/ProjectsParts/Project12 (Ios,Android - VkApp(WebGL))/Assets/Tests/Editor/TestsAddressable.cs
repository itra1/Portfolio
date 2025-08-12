using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Engine.Scripts.Managers.Libraries;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.TestTools;

namespace Tests.Editor
{
	[TestFixture]
	public class TestsAddressable
	{
		//[UnityTest]
		//public IEnumerator RecordCatalog()
		//{
		//	CatalogProcessor.RecordCatalog(new Engine.Editor.Scripts.AddressableBuilds.Data.CatalogData());
		//	yield return null;
		//}
		//[UnityTest]
		//public IEnumerator RequestCatalog()
		//{
		//	yield return CatalogLoader.LoadCatalog((result) =>
		//	{
		//		Debug.Log(result);
		//	});
		//	yield return null;
		//}

		[UnityTest]
		public IEnumerator LoadLocalAddressable()
		{
			var opHandle = Addressables.LoadAssetAsync<TimelinesLibrary>("TimelinesLibrary");
			yield return opHandle;

			var array = opHandle.Result;

			Debug.Log("Type: " + array.TimelinesList.Count);
		}

		[UnityTest]
		public IEnumerator LoadRemoteAddressable()
		{
			var opHandle = Addressables.LoadAssetAsync<TimelinesLibrary>("SergeiTimelinesLibrary");
			yield return opHandle;

			var array = opHandle.Result;

			Debug.Log("Count tracks: " + array.TimelinesList.Count);
		}

		[UnityTest]
		public IEnumerator LoadNoExistsRemoteAddressable()
		{
			yield return Addressables.InitializeAsync();
			//yield return Addressables.LoadContentCatalogAsync("https://netarchitect.ru/realgames/addressable_musictap/main/WebGL/catalog_main.json", true);
			//var l = Addressables.ResourceManager.ResourceProviders;

			//foreach (UnityEngine.ResourceManagement.ResourceProviders.IResourceProvider item in l)
			//{
			//	Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(item));
			//}
			//yield return null;


			var resourceLocators = Addressables.ResourceLocators;
			foreach (IResourceLocator locator in resourceLocators)
			{
				//Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(Addressables.GetLocatorInfo(locator.LocatorId)));
				var p = Addressables.GetLocatorInfo(locator.LocatorId);
				Debug.Log(p.Locator.Keys.Contains("tt"));
			}

			yield break;
			var opHandle = Addressables.LoadAssetAsync<TimelinesLibrary>("tt");
			yield return opHandle;

			if (opHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
				yield break;

			var array = opHandle.Result;

			Debug.Log("Count tracks: " + array.TimelinesList.Count);
		}

		[UnityTest]
		public IEnumerator LoadListRemoteAddressable()
		{
			string[] keys = new string[] { "TimelinesLibrary", "SergeiTimelinesLibrary", "tt" };
			var opHandle = Addressables.LoadAssetsAsync<TimelinesLibrary>(keys, obj => { });
			yield return opHandle;

			var array = opHandle.Result;

			Debug.Log("Count tracks: " + array.Count);
		}

		[Test]
		public void TestPath()
		{
			Debug.Log($"exec: {System.Guid.NewGuid().ToString()}");
		}

		[UnityTest]
		public IEnumerator SetResourcesLocator()
		{
			yield return CheckResourcesLocator();
		}

		[UnityTest]
		public IEnumerator CheckResourcesLocator()
		{
			yield return Addressables.InitializeAsync();

			Addressables.ClearResourceLocators();
			//while (Addressables.ResourceLocators.Count() > 0)
			//	Addressables.RemoveResourceLocator(Addressables.ResourceLocators.First());

			var resourceLocators = Addressables.ResourceLocators;
			Debug.Log($"Locators count {resourceLocators.Count()}");
			foreach (IResourceLocator locator in resourceLocators)
			{
				ResourceLocatorInfo locatorInfo = Addressables.GetLocatorInfo(locator.LocatorId);
				if (locatorInfo != null && locatorInfo.CatalogLocation != null)
				{
					if (locatorInfo.CanUpdateContent)
						Debug.Log($"Locator {locator.LocatorId} was loaded from an UPDATABLE catalog with internal id : {locatorInfo.CatalogLocation.InternalId}");
					else
						Debug.Log($"Locator {locator.LocatorId} was loaded from an NON-UPDATABLE catalog with internal id : {locatorInfo.CatalogLocation.InternalId}");
				}
				else
				{
					Debug.Log($"Locator {locator.LocatorId} is not associated with a catalog");
				}
			}
		}

		public IEnumerator AddIfNotExists(string url)
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
				var handler = Addressables.LoadContentCatalogAsync(url, true);
				yield return handler;
			}
		}
	}
}