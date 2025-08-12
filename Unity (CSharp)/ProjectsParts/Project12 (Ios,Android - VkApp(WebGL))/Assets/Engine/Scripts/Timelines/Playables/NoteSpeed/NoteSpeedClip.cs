using System;
using Engine.Scripts.Managers;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Engine.Scripts.Timelines.Playables.NotesSpeed
{
	[Serializable]
	public class NoteSpeedClip : PlayableAsset, ITimelineClipAsset
	{
		[SerializeField][HideInInspector] private double _inTime;
		[SerializeField][HideInInspector] private double _outTime;
		[SerializeField][HideInInspector] private RhythmTimelineAsset _parentAsset;
		[SerializeField] private float _speed = 1;

		public float Speed => _speed;
		public ClipCaps clipCaps => ClipCaps.None;

		public double InTime => _inTime;
		public double OutTime => _outTime;
		public RhythmTimelineAsset ParentAsset => _parentAsset;

		public void SetTimeIn(double time) => _inTime = time;
		public void SetTimeOut(double time) => _outTime = time;
		public void SetParentAsset(RhythmTimelineAsset asset) => _parentAsset = asset;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			var beh = new NoteSpeedBehaviour()
			{
				NoteSpeed = this
			};
			ScriptPlayable<NoteSpeedBehaviour> playable = ScriptPlayable<NoteSpeedBehaviour>.Create(graph, beh);

			return playable;
		}
	}
}
