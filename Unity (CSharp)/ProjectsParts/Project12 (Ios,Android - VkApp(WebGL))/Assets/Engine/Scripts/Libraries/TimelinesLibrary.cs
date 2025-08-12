using System.Collections.Generic;
using Engine.Scripts.Timelines;
using UnityEngine;

namespace Engine.Scripts.Managers.Libraries
{
	[CreateAssetMenu(fileName = "TimelinesLibrary", menuName = "ScriptableObjects/TimelinesLibrary")]
	public class TimelinesLibrary : ScriptableObject, ITimelinesLibrary
	{
		[SerializeField] private List<RhythmTimelineAsset> _timelinesList;

		public List<RhythmTimelineAsset> TimelinesList => _timelinesList;
	}
}
