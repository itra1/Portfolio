namespace Core.User
{
	/// <summary>
	/// Устаревшее название - "User"
	/// Данный интерфейс обеспечивает доступ к свойствам только для записи данных
	/// </summary>
	public interface IUserProfileSetter
	{
		int Id { set; }
		string FirstName { set; }
		string LastName { set; }
		string Email { set; }
		string AvatarUrl { set; }
	}
}