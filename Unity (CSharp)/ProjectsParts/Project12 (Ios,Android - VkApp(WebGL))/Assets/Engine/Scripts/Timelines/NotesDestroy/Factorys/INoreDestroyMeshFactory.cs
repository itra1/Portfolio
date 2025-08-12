using UnityEngine;

namespace Engine.Assets.Engine.Scripts.Timelines.NotesDestroy.Factorys
{
	public interface INoreDestroyMeshFactory
	{
		void FreeInstance(NotePartsMesh note);
		NotePartsMesh GetInstance(Texture2D tex, Transform parent);
	}
}