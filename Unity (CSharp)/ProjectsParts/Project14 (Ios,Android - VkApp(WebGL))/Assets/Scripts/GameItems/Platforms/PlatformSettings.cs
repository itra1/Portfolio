using System.Collections.Generic;
using Scripts.GameItems.Platforms;
using UnityEngine;
using Utils;

namespace Game.GameItems.Platforms
{
	[System.Serializable]
	public class PlatformSettings
	{
		[SerializeField] private float _platformDistance;
		[SerializeField] private IntRange _platformLevelSpawn;
		[SerializeField] private List<LevelSpawnSettings> _levelSpawn;

		public float PlatformDistance => _platformDistance;
		public IntRange PlatformLevelSpawn => _platformLevelSpawn;
		public List<LevelSpawnSettings> LevelSpawn => _levelSpawn;

		[System.Serializable]
		public class LevelSpawnSettings
		{
			public int Level;
			public List<PlatformFormationItem> Platforms;
		}
	}
}
