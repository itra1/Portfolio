namespace Core.Configs
{
	/// <summary>
	/// Устаревшее название - "ConfigManager"
	/// </summary>
	public interface IConfig
	{
		bool IsLoaded { get; }
		bool ContainsKey(string key);
		bool TryGetValue(string key, out string value);
		string GetValue(string key);
	}
}