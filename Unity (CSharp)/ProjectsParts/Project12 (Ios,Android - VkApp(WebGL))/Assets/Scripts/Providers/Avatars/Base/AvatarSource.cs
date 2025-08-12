using UnityEngine;
using Uuid;

namespace Game.Scripts.Providers.Avatars.Base
{

	[System.Serializable]
	public struct AvatarSource
	{
		[UUID] public string Uuid;
		public Sprite Avatar50Round10;
		public bool IsMale;
	}
}
