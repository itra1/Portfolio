using System;

namespace Game.Scripts.Providers.Profiles.Common
{
	public class Profile : IProfile
	{
		public DateTime FirstLogin { get; set; } = DateTime.MinValue;
		public string UserName { get; set; }
		public string AvatarUuid { get; set; }
		public int Points { get; set; }
		public int Level { get; set; }
		public int Balance { get; set; }
		public int Stars { get; set; }
		public int StarsInLevel { get; set; }
	}
}
