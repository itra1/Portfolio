using UnityEngine;

namespace Engine.Assets.Engine.Scripts.Timelines.NotesDestroy.Factorys
{
	public interface INoteDestroyFactory
	{
		void FreeInstance(NotePartsDestroy note);
		NotePartsDestroy GetInstance(Texture2D tex, Transform parent);
	}
}