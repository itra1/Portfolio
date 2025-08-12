using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Game.Scripts.Providers.Addressable.Common
{
	internal class ResourceCustomLocator : IResourceLocator
	{
		public string LocatorId => throw new NotImplementedException();

		public IEnumerable<object> Keys => throw new NotImplementedException();

		public IEnumerable<IResourceLocation> AllLocations => throw new NotImplementedException();

		public bool Locate(object key, Type type, out IList<IResourceLocation> locations)
		{
			throw new NotImplementedException();
		}
	}
}
