using Engine.Scripts.Inputs;
using Engine.Scripts.Timelines.Notes.Base;
using Engine.Scripts.Timelines.Notes.Common;
using Engine.Scripts.Timelines.Playables;
using UnityEngine.Events;

namespace Engine.Scripts.Managers
{
	public interface IRhythmProcessor
	{
		UnityEvent<InputEventData> OnTriggerInputEvent { get; set; }
		UnityEvent<NoteTriggerEventData> OnNoteTriggerEvent { get; set; }
		UnityEvent<Note> OnNoteInitializeEvent { get; set; }
		UnityEvent<Note> OnNoteResetEvent { get; set; }
		UnityEvent<Note> OnNoteActivateEvent { get; set; }
		UnityEvent<Note> OnNoteDeactivateEvent { get; set; }
		IRhythmDirector RhythmDirector { get; }

		Note CreateNewNote(NoteDefinition noteDefinition, RhythmClip rhythmClip);
		void DestroyNote(Note note);
		void SetRhythmDirector(IRhythmDirector rhythmDirector);
		void TriggerInput(InputEventData inputEventData);
	}
}