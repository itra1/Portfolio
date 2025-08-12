using System;

namespace Game.Scripts.Providers.Profiles.Common
{
	public interface IProfile
	{
		string UserName { get; set; }
		string AvatarUuid { get; set; }
		int Points { get; set; }
		int Level { get; set; }
		int Balance { get; set; }
		int StarsInLevel { get; set; }
		int Stars { get; set; }
		DateTime FirstLogin { get; set; }
	}
}