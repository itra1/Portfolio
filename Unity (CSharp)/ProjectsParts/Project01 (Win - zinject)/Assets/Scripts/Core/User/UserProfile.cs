namespace Core.User
{
	/// <summary>
	/// Устаревшее название - "User"
	/// </summary>
	public class UserProfile : IUserProfile, IUserProfileSetter
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string AvatarUrl { get; set; }
	}
}