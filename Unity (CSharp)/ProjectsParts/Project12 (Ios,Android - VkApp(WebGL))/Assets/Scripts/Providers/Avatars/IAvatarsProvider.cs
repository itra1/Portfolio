using System.Collections.Generic;
using Game.Scripts.Providers.Avatars.Base;
using Game.Scripts.App;
using UnityEngine;

namespace Game.Scripts.Providers.Avatars
{
	public interface IAvatarsProvider : IApplicationLoaderItem
	{
		List<AvatarSource> Avatars { get; }
		List<AvatarSource> MaleAvatars { get; }
		List<AvatarSource> FemaleAvatars { get; }

		string RandomAvatarUuid();
		Sprite GetAvatar(string uuid);
		AvatarSource GetRandom();
	}
}