using Engine.Scripts.Timelines.Notes.Base;
using Engine.Scripts.Timelines.Notes.Common;
using UnityEngine;

namespace Engine.Scripts.Timelines.Notes.Factorys
{
	public interface INoteFactory
	{
		void FreeAll();
		void FreeInstance(Note note);
		Note GetInstance(NoteDefinition noteDefinition, int trackId, Transform parent);
	}
}