using Core.Engine.Components.SaveGame;

namespace Core.Engine.Components.User
{
	public class UserProviderSave :SaveProperty<UserSaver>
	{
		public override UserSaver DefaultValue => new()
		{
			Name = "",
			Avatar = "0",
			Age = 7,
			Points = 0
		};
	}
	public class UserSaver
	{
		public string Name;
		public string Avatar;
		public int Age;
		public ulong Points;
	}
}
