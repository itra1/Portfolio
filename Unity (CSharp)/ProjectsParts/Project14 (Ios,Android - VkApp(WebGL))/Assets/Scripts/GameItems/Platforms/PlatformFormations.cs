using System.Collections.Generic;
using System.Linq;
using Core.Engine.Factorys.Base;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Scripts.GameItems.Platforms
{
#if UNITY_EDITOR

	[CustomEditor(typeof(PlatformFormations)), CanEditMultipleObjects]
	public class PlatformFormationsEditor :Editor
	{
		private int _index;
		private float _degreeRotate;
		private void OnEnable()
		{
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			_ = EditorGUILayout.BeginHorizontal();

			_index = EditorGUILayout.IntField("Index", _index);
			if (GUILayout.Button("Set formation"))
			{
				for (int i = 0; i < targets.Length; i++)
				{
					var _script = (PlatformFormations)targets[i];
					_script.SetFormation(_index);
				}
			}
			EditorGUILayout.EndHorizontal();

			_ = EditorGUILayout.BeginHorizontal();
			_degreeRotate = EditorGUILayout.FloatField("Degree", _degreeRotate);
			if (GUILayout.Button("Rotate"))
			{
				var angle = ((PlatformFormations)targets[0]).transform.localEulerAngles;
				for (int i = targets.Length - 1; i >= 0; i--)
				{
					var _script = (PlatformFormations)targets[i];
					_script.transform.localEulerAngles = angle;
					angle.y += _degreeRotate;
				}
			}
			EditorGUILayout.EndHorizontal();

		}
	}

#endif
	public partial class PlatformFormations :MonoBehaviour, IFactoryInstantiateAfter
	{
		[SerializeField] private Transform _platformParent;
		[SerializeField] private Transform _platformGen;
		[SerializeField] private List<PlatformElementsGroup> _formations = new();

		private string _activeFormationUUID;

		private readonly List<PlatformElement> _items = new();
		private readonly List<PlatformElement> _activeItems = new();

		private List<PlatformElement> _prefabs = new();

		public List<PlatformElement> ActiveItems => _activeItems;

		public List<PlatformElementsGroup> Formations { get => _formations; set => _formations = value; }
		public string ActiveFormationUUID => _activeFormationUUID;

		public void SetRandomFormation()
		{
			SetFormation(UnityEngine.Random.Range(0, _formations.Count - 1));
		}

		public PlatformElementsGroup GetFormation(string uuid)
		{
			return _formations.Find(x => x.Uuid == uuid);
		}

		public PlatformElementsGroup GetRandomFormation()
		{
			return _formations[UnityEngine.Random.Range(0, _formations.Count - 1)];
		}

		public void SetRandomRotation()
		{
			SetRotation(Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0));
		}

		public void SetRotation(Quaternion rotate)
		{
			transform.rotation = rotate;
		}

		public void SetFormation(string uuid)
		{
			var targetformation = _formations.FindIndex(x => x.Uuid == uuid);
			if (targetformation == -1)
				targetformation = 0;
			SetFormation(targetformation);
		}

		public void SetFormation(int val)
		{
			if (_prefabs.Count == 0)
				FindPrefabs();

			for (int i = 0; i < _prefabs.Count; i++)
				_prefabs[i].transform.SetParent(_platformParent);

			var targetformation = _formations[val];
			SetFormation(targetformation);
		}

		public void SetFormation(PlatformElementsGroup targetformation)
		{
			RemoveComponents();

			_activeFormationUUID = targetformation.Uuid;

			for (var i = 0; i < targetformation.Platforms.Count; i++)
			{
				var targetElement = GetElement(targetformation.Platforms[i].UUID);
				targetElement.transform.localPosition = Vector3.zero;
				targetElement.transform.localRotation = targetformation.Platforms[i].Rotation;
				targetElement.gameObject.SetActive(true);
			}
		}

		[ContextMenu("Clear")]
		private void RemoveComponents()
		{

			for (int i = 0; i < _activeItems.Count; i++)
			{
#if UNITY_EDITOR
				DestroyImmediate(_activeItems[i].gameObject);
#else
				Destroy(_activeItems[i].gameObject);
#endif
			}

			_activeItems.Clear();

			while (_platformGen.childCount > 0)
				DestroyImmediate(_platformGen.GetChild(0).gameObject);
		}

		private PlatformElement GetElement(string uuid)
		{
			var pe = _items.Find(x => x.Uuid == uuid && !x.gameObject.activeSelf);

			if (!pe)
			{
				var pref = _prefabs.Find(x => x.Uuid == uuid);
				pe = Instantiate(pref.gameObject, _platformGen).GetComponent<PlatformElement>();
				pe.transform.localRotation = Quaternion.identity;
				pe.transform.localScale = Vector3.one;
				pe.transform.localPosition = Vector3.zero;

				_activeItems.Add(pe);
			}

			return pe;
		}

		private void FindPrefabs()
		{
			_prefabs = _platformParent.transform.GetComponentsInChildren<PlatformElement>(true).ToList();
		}

		public void AfterFactoryCreate()
		{
			FindPrefabs();
		}

#if UNITY_EDITOR

		[ContextMenu("Save")]
		public void SaveFormation()
		{
			var platformsArray = _platformGen.GetComponentsInChildren<PlatformElement>();

			PlatformElementsGroup peg = new()
			{
				Uuid = System.Guid.NewGuid().ToString(),
				Platforms = new()
			};
			for (var i = 0; i < platformsArray.Length; i++)
			{
				var itm = platformsArray[i];
				peg.Platforms.Add(new()
				{
					UUID = itm.Uuid,
					Rotation = itm.transform.localRotation
				});

				if (itm.IsDamage)
					peg.ExistsFamage = true;
			}
			_formations.Add(peg);
		}

		[ContextMenu("SetFormation")]
		public void SetFormation()
		{
			LoadFormation(6);
		}

		public void LoadFormation(int index)
		{
			while (_platformGen.childCount > 0)
				DestroyImmediate(_platformGen.GetChild(0).gameObject);

			var platformList = _platformParent.GetComponentsInChildren<PlatformElement>(true).ToList();
			var format = _formations[index];

			for (int i = 0; i < format.Platforms.Count; i++)
			{

				PlatformFormationItem p = format.Platforms[i];

				var platformPrefab = platformList.Find(x => x.Uuid == p.UUID);

				var instance = Instantiate(platformPrefab, _platformGen);
				instance.transform.localPosition = Vector3.zero;
				instance.transform.rotation = p.Rotation;
				instance.gameObject.SetActive(true);
			}
		}

#endif

	}
}
