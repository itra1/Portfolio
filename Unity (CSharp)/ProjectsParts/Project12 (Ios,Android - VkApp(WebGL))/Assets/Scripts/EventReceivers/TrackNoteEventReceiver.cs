using Engine.Scripts.Base;
using Engine.Scripts.Managers;
using Engine.Scripts.Timelines.Notes.Base;
using Engine.Scripts.Timelines.Notes.Common;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.EventReceivers
{
	public class TrackNoteEventReceiver : MonoBehaviour, IInjection
	{
		[Tooltip("The ID of the track to listen to.")]
		[SerializeField] protected int m_TrackID = -1;
		[Tooltip("Optionally the track object instead of the track ID.")]
		[SerializeField] protected TrackObject m_TrackObject;
		[Tooltip("Event when an note is activated on that track.")]
		[SerializeField] protected UnityEvent m_OnNoteActivate;
		[Tooltip("Event when a note is deactivated on that track.")]
		[SerializeField] protected UnityEvent m_OnNoteDeactivate;
		[Tooltip("Event when the note is triggered.")]
		[SerializeField] protected UnityEvent m_OnNoteTriggered;
		[Tooltip("Event when the note is missed.")]
		[SerializeField] protected UnityEvent m_OnNoteTriggeredMiss;
		[SerializeField] protected UnityEvent m_OnNoteTriggeredEarlyDestroy;

		protected IRhythmDirector m_RhythmDirector;

		[Inject]
		private void Constructor(IRhythmDirector rhythmDirector)
		{
			m_RhythmDirector = rhythmDirector;
			m_RhythmDirector.RhythmProcessor.OnNoteActivateEvent.AddListener(HandleOnNoteActivateEvent);
			m_RhythmDirector.RhythmProcessor.OnNoteDeactivateEvent.AddListener(HandleOnNoteDeactivateEvent);
			m_RhythmDirector.RhythmProcessor.OnNoteTriggerEvent.AddListener(HandleOnNoteTriggeredEvent);

			if (m_TrackObject == null)
			{ return; }

			for (int i = 0; i < m_RhythmDirector.TrackObjects.Length; i++)
			{
				if (m_RhythmDirector.TrackObjects[i] == m_TrackObject)
				{
					m_TrackID = i;
					break;
				}
			}
		}

		private void Start()
		{
		}

		private void HandleOnNoteActivateEvent(Note note)
		{
			if (m_TrackID != -1 && m_TrackID != note.RhythmClipData.TrackID)
			{ return; }


			m_OnNoteActivate.Invoke();
		}

		private void HandleOnNoteDeactivateEvent(Note note)
		{
			if (m_TrackID != -1 && m_TrackID != note.RhythmClipData.TrackID)
			{ return; }

			m_OnNoteDeactivate.Invoke();
		}

		private void HandleOnNoteTriggeredEvent(NoteTriggerEventData noteTriggerEventData)
		{
			if (m_TrackID != -1 && m_TrackID != noteTriggerEventData.Note.RhythmClipData.TrackID)
			{ return; }


			switch (noteTriggerEventData.EventType)
			{
				case NoteEventType.Miss:
					m_OnNoteTriggeredMiss.Invoke();
					break;

				case NoteEventType.Early:
					m_OnNoteTriggeredEarlyDestroy.Invoke();
					break;

				default:
					m_OnNoteTriggered.Invoke();
					break;
			}
		}
	}
}