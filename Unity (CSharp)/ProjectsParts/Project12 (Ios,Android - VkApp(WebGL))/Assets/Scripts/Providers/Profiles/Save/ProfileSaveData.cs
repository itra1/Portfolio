using System;
using Game.Scripts.Providers.Profiles.Common;
using Game.Scripts.Providers.Saves.Data;

namespace Game.Scripts.Providers.Profiles.Save
{
	[Serializable]
	public class ProfileSaveData : SaveProperty<Profile>
	{
		public override Profile DefaultValue => new();
	}
}
