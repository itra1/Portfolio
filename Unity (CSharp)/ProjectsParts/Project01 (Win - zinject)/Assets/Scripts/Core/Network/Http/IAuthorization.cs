namespace Core.Network.Http
{
	/// <summary>
	/// Устаревшее название - "RestManager"
	/// </summary>
	public interface IAuthorization
	{
		void Authorize(string login = null, string password = null, string role = null);
	}
}