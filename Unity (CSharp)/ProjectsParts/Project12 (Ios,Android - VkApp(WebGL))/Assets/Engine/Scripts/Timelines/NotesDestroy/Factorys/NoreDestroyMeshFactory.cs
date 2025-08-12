using System.Collections.Generic;
using Engine.Scripts.Settings;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Engine.Assets.Engine.Scripts.Timelines.NotesDestroy.Factorys
{
	public class NoreDestroyMeshFactory : INoreDestroyMeshFactory
	{
		private readonly Dictionary<string, List<NotePartsMesh>> _instancesDict = new();
		private readonly DiContainer _diContainer;
		private readonly INotesSettings _notesSettings;
		private List<NotePartsMesh> _prefabs = new();

		public NoreDestroyMeshFactory(DiContainer container, INotesSettings notesSettings)
		{
			_diContainer = container;
			_notesSettings = notesSettings;

			LoadResources();
		}

		private void LoadResources()
		{
			var notesArray = Resources.LoadAll<NotePartsMesh>(_notesSettings.NotesInResources);

			_prefabs.Clear();
			foreach (var item in notesArray)
			{
				_prefabs.Add(item);
			}
		}

		public NotePartsMesh GetInstance(Texture2D tex, Transform parent)
		{
			if (_prefabs.IsEmpty())
				LoadResources();

			string targetName = _prefabs[Random.Range(0, _prefabs.Count)].name;

			if (_instancesDict.ContainsKey(targetName))
				_instancesDict.Add(targetName, new());

			NotePartsMesh instance = _instancesDict[targetName].Find(x => !x.gameObject.activeSelf);

			if (instance == null)
			{
				var prefab = _prefabs.Find(x => x.name == targetName);
				instance = MonoBehaviour.Instantiate(prefab, parent);
				_diContainer.Inject(instance);
				_instancesDict[targetName].Add(instance);
			}
			instance.SetTexture(tex);
			instance.gameObject.SetActive(true);

			return instance;
		}

		public void FreeInstance(NotePartsMesh note)
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
			foreach (var item in _instancesDict)
			{
				foreach (var item1 in item.Value)
				{
					FreeInstance(item1);
				}
			}
		}
	}
}
