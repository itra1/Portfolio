using Game.Providers.Profile.Models;
using Game.Providers.Saves.Data;

namespace Game.Providers.Profile.Save
{
	public class ProfileProviderSave : SaveProperty<ProfileModel>
	{
		public override ProfileModel DefaultValue => new()
		{
			Name = "",
			WelcomeShow = false
		};

	}
}
