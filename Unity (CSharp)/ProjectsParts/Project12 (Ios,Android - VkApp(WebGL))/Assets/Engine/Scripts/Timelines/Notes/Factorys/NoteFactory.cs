using System.Collections.Generic;
using Engine.Scripts.Base;
using Engine.Scripts.Settings;
using Engine.Scripts.Timelines.Notes.Base;
using Engine.Scripts.Timelines.Notes.Common;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Engine.Scripts.Timelines.Notes.Factorys
{
	public class NoteFactory : INoteFactory
	{
		private readonly Dictionary<string, List<Note>> _notesDict = new();
		private DiContainer _diContainer;
		private INotesSettings _notesSettings;
		private List<Note> _prefabs = new();

		public NoteFactory(DiContainer container, INotesSettings notesSettings)
		{
			_diContainer = container;
			_notesSettings = notesSettings;

			LoadResources();
		}

		private void LoadResources()
		{
			var notesArray = Resources.LoadAll<Note>(_notesSettings.NotesInResources);

			_prefabs.Clear();
			foreach (var item in notesArray)
			{
				_prefabs.Add(item);
			}
		}

		public Note GetInstance(NoteDefinition noteDefinition, int trackId, Transform parent)
		{
			if (_prefabs.IsEmpty())
				LoadResources();

			var graphicType = _notesSettings.TrackDefaultColor[trackId];
			var targetGraphic = _notesSettings.Graphics.Find(x => x.NoteColorType == graphicType);

			Note note = null;
			if (!Application.isPlaying)
			{
				note = MakeInstance(noteDefinition.NoteType, parent);
				note.SetGraphic(targetGraphic);
				return note;
			}

			var prefabType = noteDefinition.NoteType;

			if (!_notesDict.ContainsKey(prefabType))
				_notesDict.Add(prefabType, new List<Note>());

			note = _notesDict[prefabType].Find(x => !x.gameObject.activeSelf);

			if (note == null)
			{
				note = MakeInstance(noteDefinition.NoteType, parent);
				_notesDict[prefabType].Add(note);
			}
			note.SetGraphic(targetGraphic);
			note.gameObject.SetActive(true);

			return note;
		}

		private Note MakeInstance(string type, Transform parent)
		{
			var prefab = _prefabs.Find(x => x.Type == type);
			var instance = GameObject.Instantiate(prefab, parent);
			instance.transform.position = new Vector3(0, 100, 0);

			Note note = instance.GetComponent<Note>();
			_diContainer.Inject(note);

			if (Application.isPlaying)
			{
				var components = note.GetComponentsInChildren<IInjection>();

				foreach (var item in components)
				{
					_diContainer.Inject(item);
				}
			}
			return note;
		}

		public void FreeInstance(Note note)
		{
			if (!Application.isPlaying)
			{
				GameObject.DestroyImmediate(note.gameObject);
				return;
			}
			note.gameObject.SetActive(false);
		}

		public void FreeAll()
		{
			foreach (var item in _notesDict)
			{
				foreach (var item1 in item.Value)
				{
					FreeInstance(item1);
				}
			}
		}
	}
}
