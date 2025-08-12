using Game.Providers.Audio.Base;
using StringDrop;
using System.Collections.Generic;

namespace Game.Game.Settings {
	[System.Serializable]
	public class BoardsSettings {
		public List<BoardItem> Boards;

		[System.Serializable]
		public class BoardItem {
			[StringDropList(typeof(BoardNames))] public string Name;
			[StringDropList(typeof(SoundNames))] public string HitSound;
			[StringDropList(typeof(SoundNames))] public string CrashSound;
		}
	}
}
