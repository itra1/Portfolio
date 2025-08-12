using System;
using Engine.Scripts.Timelines.Notes.Base;
using Engine.Scripts.Timelines.Notes.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace Engine.Scripts.Timelines.Playables
{
	[Serializable]
	public class RhythmBehaviour : PlayableBehaviour
	{
		[SerializeField] private NoteDefinition m_NoteDefinition;
		[SerializeField] private bool _specialPoint;
		[SerializeField] private bool _endPoint;

		protected bool _isNoteSpawned;
		protected bool _missingDefinition;
		protected Note _note;

		public NoteDefinition NoteDefinition => m_NoteDefinition;
		public RhythmClip RhythmClip { get; set; }
		public RhythmClipData RhythmClipData => RhythmClip.RhythmClipData;

		public bool SpecialPoint { get => _specialPoint; set => _specialPoint = value; }
		public bool EndPoint { get => _endPoint; set => _endPoint = value; }

		public override void OnPlayableCreate(Playable playable)
		{ }

		public void SetNoteDefinition(NoteDefinition noteDefinition)
		{
			m_NoteDefinition = noteDefinition;
		}

		//Takes care of instantiating the GameObject
		public override void OnGraphStart(Playable playable)
		{
			//_isNoteSpawned = false;
			_missingDefinition = m_NoteDefinition == null || string.IsNullOrEmpty(m_NoteDefinition.NoteType);
			if (_missingDefinition)
			{
				Debug.LogWarning($"The Rhythm Object Definition {m_NoteDefinition} for this clip is missing, or its prefab is missing or the prefab does note contain a Note component.");
			}
		}

		protected virtual void SpawnNote()
		{
			if (_missingDefinition)
				return;
			if (_isNoteSpawned)
				return;

			_note = RhythmClipData.RhythmDirector.RhythmProcessor.CreateNewNote(m_NoteDefinition, RhythmClip);
			_note.OnRemoveVisible.AddListener(RemoveNote);
			if (_endPoint)
			{
				_note.SetIsFinalPoint();
			}
			else if (_specialPoint)
			{
				_note.SetIsSpecialPoint();
			}
			_isNoteSpawned = true;
		}

		protected virtual void RemoveNote()
		{
			if (!_isNoteSpawned)
				return;

			if (!Application.isPlaying)
			{
				_isNoteSpawned = false;
			}

			if (_note != null)
				RhythmClipData.RhythmDirector.RhythmProcessor.DestroyNote(_note);
			_note = null;
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (!_isNoteSpawned)
				return;

			if (_note != null)
				_note.OnClipStart();
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (!Application.isPlaying)
				return;

			var duration = playable.GetDuration();
			var time = playable.GetTime();
			var count = time + info.deltaTime;

			if (
				(info.effectivePlayState == PlayState.Paused && count > duration)
				|| Mathf.Approximately((float) time, (float) duration)
			)
			{
				if (_note != null)
					_note.OnClipStop();
			}
		}

		public void MixerProcessFrame(Playable thisPlayable, FrameData info, object playerData, double timelineCurrentTime)
		{
			if (_missingDefinition)
				return;
			/* Calculate the clip time starting from the actual Timeline time
			the only reason why we need this is because we need it to be able to be negative or past the clip's duration,
			so we can handle bullets also after the clip ends
			thisPlayable.GetTime() only gives time constrained to the clip duration */
#if UNITY_EDITOR
			// Update the BPM in case it was changed in the inspector.
			if (RhythmClipData.RhythmDirector == null)
				return;
#endif

			var globalClipStartTime = timelineCurrentTime - RhythmClipData.ClipStart;
			var globalClipEndTime = timelineCurrentTime - RhythmClipData.ClipEnd;

			var timeRange = RhythmClipData.RhythmDirector.SpawnTimeRange;

			if ((!(globalClipStartTime >= -timeRange.Max) && !Application.isPlaying) || !(globalClipEndTime < timeRange.Min))
			{
				if (_isNoteSpawned)
					RemoveNote();

				return;
			}

			if ((globalClipStartTime >= -timeRange.Max) && (globalClipEndTime < timeRange.Min) && !_isNoteSpawned)
			{
				SpawnNote();
			}

			if (_isNoteSpawned && _note != null)
				_note.TimelineUpdate(globalClipStartTime, globalClipEndTime);
		}

		public override void OnGraphStop(Playable playable)
		{
		}

		//Takes care of destroying the GameObject, if it still exists
		public override void OnPlayableDestroy(Playable playable)
		{
			RemoveNote();
		}
	}
}
