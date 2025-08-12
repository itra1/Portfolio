using System.Collections.Generic;
using Engine.Assets.Engine.Scripts.Timelines.NotesDestroy;
using Engine.Engine.Scripts.Settings.Common;
using StringDrop;
using UnityEngine;

namespace Engine.Scripts.Settings
{
	[CreateAssetMenu(fileName = "NotesSettings", menuName = "Settings/Notes/NotesSettings")]
	public class NotesSettings : ScriptableObject, INotesSettings
	{
		[SerializeField] private string _notesInResources;
		[SerializeField] private string _noteMeshes;
		[SerializeField] private NotePartsDestroy _noteDestroyPrefab;
		[SerializeField][StringDropList(typeof(NoteColorType), false)] private List<string> _trackDefaultColor;
		[SerializeField] private List<NoteGraphic> _graphics;

		public string NotesInResources => _notesInResources;
		public List<NoteGraphic> Graphics => _graphics;
		public List<string> TrackDefaultColor => _trackDefaultColor;
		public string NoteMeshes => _noteMeshes;
		public NotePartsDestroy NoteDestroyPrefab => _noteDestroyPrefab;
	}
}
