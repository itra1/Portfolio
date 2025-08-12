using System.Collections.Generic;
using Game.Scripts.Providers.Avatars.Base;
using UnityEngine;

namespace Game.Scripts.Providers.Avatars.Settings
{
	[System.Serializable]
	public class AvatarsSettings
	{
		[SerializeField] private List<AvatarSource> _avatars;
		public List<AvatarSource> Avatars => _avatars;
	}
}
