using System.Collections.Generic;
using Engine.Assets.Engine.Scripts.Timelines.NotesDestroy;
using Engine.Engine.Scripts.Settings.Common;

namespace Engine.Scripts.Settings
{
	public interface INotesSettings
	{
		string NotesInResources { get; }
		List<NoteGraphic> Graphics { get; }
		List<string> TrackDefaultColor { get; }
		string NoteMeshes { get; }
		NotePartsDestroy NoteDestroyPrefab { get; }
	}
}