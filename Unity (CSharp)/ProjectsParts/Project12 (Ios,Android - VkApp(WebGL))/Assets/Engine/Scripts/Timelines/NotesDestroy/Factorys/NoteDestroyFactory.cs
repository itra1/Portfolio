using System.Collections.Generic;
using Engine.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace Engine.Assets.Engine.Scripts.Timelines.NotesDestroy.Factorys
{
	public class NoteDestroyFactory : INoteDestroyFactory
	{
		private readonly DiContainer _diContainer;
		private readonly INotesSettings _notesSettings;
		private readonly INoreDestroyMeshFactory _meshFactory;
		private List<NotePartsDestroy> _instances = new();

		public NoteDestroyFactory(DiContainer container, INotesSettings notesSettings, INoreDestroyMeshFactory meshFactory)
		{
			_diContainer = container;
			_notesSettings = notesSettings;
			_meshFactory = meshFactory;
		}

		public NotePartsDestroy GetInstance(Texture2D tex, Transform parent)
		{
			NotePartsDestroy instance = _instances.Find(x => !x.gameObject.activeSelf);

			if (instance == null)
			{
				instance = MonoBehaviour.Instantiate(_notesSettings.NoteDestroyPrefab, parent);
				_instances.Add(instance);
			}
			if (instance.Mesh != null)
			{
				_meshFactory.FreeInstance(instance.Mesh);
			}
			instance.transform.SetParent(parent);
			instance.SetMesh(_meshFactory.GetInstance(tex, instance.transform));
			instance.gameObject.SetActive(true);

			return instance;
		}

		public void FreeInstance(NotePartsDestroy note)
		{
			if (!Application.isPlaying)
			{
				GameObject.DestroyImmediate(note.gameObject);
				return;
			}
			note.gameObject.SetActive(false);
		}
	}
}
