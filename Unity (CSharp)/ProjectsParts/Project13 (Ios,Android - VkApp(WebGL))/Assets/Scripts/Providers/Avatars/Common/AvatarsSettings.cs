using System;
using UnityEngine;

namespace Game.Providers.Avatars.Common {
	[Serializable]
	public class AvatarsSettings :IAvatarsSettings {
		[SerializeField] private string _avatarSettings;
		public string AvatarsFolder => _avatarSettings;
	}
}
