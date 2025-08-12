using System;

namespace Core.Materials.Loading.AutoPreloader.Info
{
	public struct MaterialDataTypeLoadingInfo
	{
		public Type Type { get; }
		public string Url { get; }

		public MaterialDataTypeLoadingInfo(Type type, string url)
		{
			Type = type;
			Url = url;
		}
	}
}