using System.Collections.Generic;
using Engine.Scripts.Timelines;
using Game.Scripts.App;
using UnityEngine;

namespace Game.Scripts.Providers.Songs
{
	public interface ISongsProvider : IApplicationLoaderItem
	{
		List<RhythmTimelineAsset> Songs { get; }

		RhythmTimelineAsset GetSong(string uuid);
		Texture2D GetCover(string songUuid);
	}
}