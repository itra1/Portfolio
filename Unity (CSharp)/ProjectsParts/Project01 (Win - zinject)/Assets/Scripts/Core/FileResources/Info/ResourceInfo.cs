using Core.FileResources.Customizing.Category;

namespace Core.FileResources.Info
{
	public struct ResourceInfo
	{
		public string Url { get; }
		public ResourceCategory Category { get; }
		public string Name { get; }
		
		public ResourceInfo(string url, 
			ResourceCategory category = ResourceCategory.File,
			string name = "")
		{
			Url = url;
			Category = category;
			Name = name;
		}
		
		public override string ToString()
		{
			return $"{{url: \"{Url}\", name: \"{Name}\", category: \"{Category}\"}}";
		}
	}
}