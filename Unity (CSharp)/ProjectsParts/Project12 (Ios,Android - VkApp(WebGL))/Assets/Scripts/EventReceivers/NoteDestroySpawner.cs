using Engine.Assets.Engine.Scripts.Timelines.NotesDestroy.Factorys;
using Engine.Scripts.Base;
using Engine.Scripts.Timelines.Notes.Base;
using UnityEngine;

namespace Game.Assets.Scripts.EventReceivers
{
	public class NoteDestroySpawner : MonoBehaviour, IInjection
	{
		private INoteDestroyFactory _factory;

		private void Constructor(INoteDestroyFactory factory)
		{
			_factory = factory;
		}

		public void Spawn()
		{
			var note = GetComponentInParent<Note>();

			var instance = _factory.GetInstance(note.Texture.sprite.texture, note.transform.parent);
			instance.Play(note.NoteSpeed);


		}
	}
}
