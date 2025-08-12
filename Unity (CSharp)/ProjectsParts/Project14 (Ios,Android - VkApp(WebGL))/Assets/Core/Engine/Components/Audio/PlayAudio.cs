using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Engine.Components.Audio
{
	public class PlayAudio : IPlayAudio
	{
		private IAudioAdapter _audioAdapter;

		public static PlayAudio Instance { get; set; }

		public PlayAudio()
		{
			Instance = this;
			_audioAdapter = new DarkTonikAdapter();
		}

		public static void PlaySound(string type)
		{
			Instance._audioAdapter?.PlaySound(type);
		}

	}
}
