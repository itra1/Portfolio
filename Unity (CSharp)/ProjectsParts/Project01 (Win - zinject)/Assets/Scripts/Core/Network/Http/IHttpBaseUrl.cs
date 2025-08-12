namespace Core.Network.Http
{
	/// <summary>
	/// Устаревшее название - "RestManager"
	/// </summary>
	public interface IHttpBaseUrl
	{
		string Server { get; }
		string ServerApi { get; }
		string ServerDoc { get; }
	}
}