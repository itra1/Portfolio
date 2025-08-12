using Engine.Scripts.Timelines.Notes.Base;
using Engine.Scripts.Timelines.Notes.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Engine.Scripts.Timelines.Notes.Components
{
	public class NoteEventReceiver : MonoBehaviour
	{
		[SerializeField] private Note _note;
		[SerializeField] private UnityEvent OnNoteActivate;
		[SerializeField] private UnityEvent OnNoteDeactivate;
		[SerializeField] private UnityEvent OnNoteTriggered;

		private void OnEnable()
		{
			FindComponents();

			if (_note == null)
				return;

			_note.OnActivate.AddListener(HandleOnNoteActivateEvent);
			_note.OnDeactivate.AddListener(HandleOnNoteDeactivateEvent);
			_note.OnNoteTriggerEvent.AddListener(HandleOnNoteTriggeredEvent);
		}

		private void FindComponents()
		{
			if (_note == null)
				_note = GetComponent<Note>();
		}

		protected virtual void HandleOnNoteActivateEvent(Note note)
		{
			OnNoteActivate.Invoke();
		}

		protected virtual void HandleOnNoteDeactivateEvent(Note note)
		{
			OnNoteDeactivate.Invoke();
		}

		protected virtual void HandleOnNoteTriggeredEvent(NoteTriggerEventData noteTriggerEventData)
		{
			switch (noteTriggerEventData.EventType)
			{
				default:
					OnNoteTriggered.Invoke();
					break;
			}
		}
	}
}