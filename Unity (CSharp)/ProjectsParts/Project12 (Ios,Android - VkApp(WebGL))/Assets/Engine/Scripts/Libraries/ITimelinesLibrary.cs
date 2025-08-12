using System.Collections.Generic;
using Engine.Scripts.Timelines;

namespace Engine.Scripts.Managers.Libraries
{
	internal interface ITimelinesLibrary
	{
		List<RhythmTimelineAsset> TimelinesList { get; }
	}
}