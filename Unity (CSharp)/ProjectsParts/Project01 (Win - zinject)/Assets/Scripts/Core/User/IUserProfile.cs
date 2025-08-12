namespace Core.User
{
	/// <summary>
	/// Устаревшее название - "User"
	/// Данный интерфейс обеспечивает доступ к свойствам только для чтения данных
	/// </summary>
	public interface IUserProfile
	{
		int Id { get; }
		string FirstName { get; }
		string LastName { get; }
		string Email { get; }
		string AvatarUrl { get; }
	}
}