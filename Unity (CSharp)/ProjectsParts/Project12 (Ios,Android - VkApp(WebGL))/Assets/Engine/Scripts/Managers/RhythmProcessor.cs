using Engine.Scripts.Common.Interfaces;
using Engine.Scripts.Inputs;
using Engine.Scripts.Timelines.Playables;
using Engine.Scripts.Timelines.Notes.Base;
using Engine.Scripts.Timelines.Notes.Common;
using Engine.Scripts.Timelines.Notes.Factorys;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Engine.Scripts.Managers
{
	/// <summary>
	/// Gets information from the RhythmDirector and from the input to processes notes.
	/// </summary>
	public class RhythmProcessor : IRhythmProcessor
	{
		[HideInInspector] public UnityEvent<InputEventData> OnTriggerInputEvent { get; set; } = new();
		[HideInInspector] public UnityEvent<NoteTriggerEventData> OnNoteTriggerEvent { get; set; } = new();
		[HideInInspector] public UnityEvent<Note> OnNoteInitializeEvent { get; set; } = new();
		[HideInInspector] public UnityEvent<Note> OnNoteResetEvent { get; set; } = new();
		[HideInInspector] public UnityEvent<Note> OnNoteActivateEvent { get; set; } = new();
		[HideInInspector] public UnityEvent<Note> OnNoteDeactivateEvent { get; set; } = new();

		private readonly DiContainer _diContainer;
		private readonly ISceneNotesParent _notesWorldParent;
		private readonly INoteFactory _noteFactory;

		public IRhythmDirector RhythmDirector { get; private set; }

		public RhythmProcessor(
			DiContainer diContainer,
			ISceneNotesParent notesWorldParent,
			INoteFactory noteFactory
			)
		{
			_diContainer = diContainer;
			_notesWorldParent = notesWorldParent;
			_noteFactory = noteFactory;
		}

		public void ClearNotes()
		{
			_noteFactory.FreeAll();
		}

		public virtual void TriggerInput(InputEventData inputEventData)
		{

			InvokeNoteInitializeEventInternal(inputEventData);

			var note = RhythmDirector.TrackObjects[inputEventData.TrackID].CurrentNote;

			if (note == null)
			{
				if (inputEventData.Tap && !inputEventData.Release)
				{
					note = RhythmDirector.TrackObjects[inputEventData.TrackID].CurrentVisibleNote;

					if (note == null)
						return;
					note.DestroyLoss();
					RhythmDirector.TrackObjects[inputEventData.TrackID].RemoveVisibleNote(note);
				}
				return;
			}

			note.OnTriggerInput(inputEventData);
		}

		protected void InvokeNoteInitializeEventInternal(InputEventData inputEventData)
		{
			OnTriggerInputEvent?.Invoke(inputEventData);
		}

		public Note CreateNewNote(NoteDefinition noteDefinition, RhythmClip rhythmClip)
		{
			var note = _noteFactory.GetInstance(noteDefinition, rhythmClip.RhythmClipData.TrackID, _notesWorldParent.NotesParent);

			if (Application.isPlaying)
				RegisterToNoteEvents(note);

			note.Initialize(rhythmClip.RhythmClipData);

			return note;
		}

		private void RegisterToNoteEvents(Note note)
		{
			note.OnNoteTriggerEvent.AddListener(HandleNoteTriggerEvent);
			note.OnInitialize.AddListener(HandleNoteInitializeEvent);
			note.OnReset.AddListener(HandleNoteResetEvent);
			note.OnActivate.AddListener(HandleNoteActivateEvent);
			note.OnDeactivate.AddListener(HandleNoteDeactivateEvent);
		}

		protected virtual void HandleNoteInitializeEvent(Note note)
		{
			InvokeNoteInitializeEventInternal(note);
		}

		protected void InvokeNoteInitializeEventInternal(Note note)
		{
			OnNoteInitializeEvent?.Invoke(note);
		}

		protected virtual void HandleNoteResetEvent(Note note)
		{
			InvokeNoteResetEventInternal(note);
		}

		protected void InvokeNoteResetEventInternal(Note note)
		{
			OnNoteResetEvent?.Invoke(note);
		}

		protected virtual void HandleNoteActivateEvent(Note note)
		{
			InvokeNotActivateEventInternal(note);
		}

		protected void InvokeNotActivateEventInternal(Note note)
		{
			OnNoteActivateEvent?.Invoke(note);
		}

		protected virtual void HandleNoteDeactivateEvent(Note note)
		{
			InvokeNoteDeactivateEventInternal(note);
		}

		protected void InvokeNoteDeactivateEventInternal(Note note)
		{
			OnNoteDeactivateEvent?.Invoke(note);
		}

		protected virtual void HandleNoteTriggerEvent(NoteTriggerEventData noteTriggerEventData)
		{
			InvokeNoteTriggerEventInternal(noteTriggerEventData);
		}

		protected void InvokeNoteTriggerEventInternal(NoteTriggerEventData noteTriggerEventData)
		{
			OnNoteTriggerEvent?.Invoke(noteTriggerEventData);
		}

		public void DestroyNote(Note note)
		{
			if (note == null)
				return;

			_noteFactory.FreeInstance(note);
		}

		public void SetRhythmDirector(IRhythmDirector rhythmDirector)
		{
			RhythmDirector = rhythmDirector;
		}
	}
}