using UnityEngine;

namespace Game.Scripts.Providers.Premiums.Settings
{
	[System.Serializable]
	public class Reward
	{
		[SerializeField] private int _songsCount;

		public int SongsCount => _songsCount;
	}
}
