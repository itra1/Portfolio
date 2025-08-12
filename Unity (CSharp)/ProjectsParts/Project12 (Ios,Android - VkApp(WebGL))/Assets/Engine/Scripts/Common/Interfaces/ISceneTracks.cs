using Engine.Scripts.Managers;
using UnityEngine;

namespace Engine.Scripts.Common.Interfaces
{
	public interface ISceneTracks
	{
		Transform TracksParent { get; }
		TrackObject[] Tracks { get; }
	}
}
