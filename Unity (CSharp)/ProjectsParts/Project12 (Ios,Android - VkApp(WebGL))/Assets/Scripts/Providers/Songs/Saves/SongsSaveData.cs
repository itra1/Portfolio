using System.Collections.Generic;
using Game.Scripts.Providers.Saves.Data;

namespace Game.Scripts.Providers.Songs.Saves
{
	public class SongsSaveData : SaveProperty<SongsSave>
	{
		public override SongsSave DefaultValue => new();
	}

	[System.Serializable]
	public class SongsSave
	{
		public List<string> OpenSongs { get; set; } = new();
		public List<SongScoreData> Score { get; set; } = new();
	}
}
