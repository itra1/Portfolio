using UnityEngine;
using System.Collections.Generic;
using it.Game.Player;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace it.Game.Environment.All.Animals
{
  public class AnimalSpawnManager : MonoBehaviourBase
  {
	 public List<SpawnPrefab> _prefabs = new List<SpawnPrefab>();
	 public List<SpawnInfo> _spawnList = new List<SpawnInfo>();

	 private static AnimalSpawnManager _instance;
	 public static AnimalSpawnManager Instance
	 {
		get
		{
		  if (_instance == null)
			 _instance = MonoBehaviour.FindObjectOfType<AnimalSpawnManager>();
		  return _instance;
		}
  }

	 private float _actualDistance;

	 private void Update()
	 {
		if (PlayerBehaviour.Instance == null)
		  return;

		SpawnInfo info;

		for (int i = 0; i < _spawnList.Count; i++)
		{
		  if (!_spawnList[i]._spawner.IsEnable)
			 continue;

		  info = _spawnList[i];
		  _actualDistance = (info.position - PlayerBehaviour.Instance.transform.position).sqrMagnitude;

		  if (!info.isSpawn && _actualDistance < info.sqrDistance)
		  {
			 info.isSpawn = true;
			 if(info._instance == null)
			 {
				info._instance = Instantiate(_prefabs.Find(x => x.uuid.Equals(info.uuid))._prefab, info._spawner.Parent);
				info._instance.gameObject.SetActive(false);
			 }
			 info._instance.transform.position = info._spawner.transform.position;
			 info._instance.gameObject.SetActive(true);
			 info._instance.transform.rotation = info._spawner.RandomRotation ? Quaternion.Euler(0, Random.Range(0f, 360f), 0) : Quaternion.Euler(info._spawner.Rotation);
			 if (info._spawner.RandomScale)
			 {
				float scale = info._spawner.ScaleSpan.Random;
				info._instance.transform.localScale = new Vector3(scale, scale, scale);
			 }
			 IAnimalSpawnerAfter[] after = info._spawner.GetComponents<IAnimalSpawnerAfter>();

			 for(int x = 0; x < after.Length; x++) {
				after[x].AfterSpawn(info._instance);
			 }

		  }
		  if (info.isSpawn && info._instance != null && _actualDistance > info.sqrDistance && _actualDistance > (info._instance.transform.position - PlayerBehaviour.Instance.transform.position).sqrMagnitude)
		  {
			 info.isSpawn = true;
			 info._instance.gameObject.SetActive(false);
		  }
		}
	 }


#if UNITY_EDITOR

	 [MenuItem("Scene/AnimalsFind")]
	 public static void SceneReady()
	 {
		var lm = MonoBehaviour.FindObjectOfType<AnimalSpawnManager>();

		if (lm == null)
		{
		  Debug.Log("No location manager");
		  return;
		}

		lm.FindSpawners();
	 }
	 [ContextMenu("Find Animals spawners")]
	 private void FindSpawners()
	 {
		AnimalSpaner[] spawners = FindObjectsOfType<AnimalSpaner>();

		_spawnList.Clear();

		for(int i = 0; i  < spawners.Length; i++)
		{
		  SpawnInfo lStawnInfo = new SpawnInfo();
		  lStawnInfo.uuid = spawners[i].AnimaUuid;
		  lStawnInfo.sqrDistance = spawners[i].Radius * spawners[i].Radius;
		  lStawnInfo.position = spawners[i].transform.position;
		  lStawnInfo._spawner = spawners[i];
		  _spawnList.Add(lStawnInfo);
		}

		_prefabs.Clear();

		GameObject[] animalsSources = Resources.LoadAll<GameObject>("Prefabs/Animals");

		for(int i = 0; i < _spawnList.Count; i++)
		{
		  SpawnPrefab pfb = _prefabs.Find(x => x.uuid == _spawnList[i].uuid);
		  if (pfb != null)
		  {
			 _spawnList[i].title = pfb._prefab.GetComponent<it.Game.NPC.NPC>().Title;
			 _spawnList[i]._spawner.AnimalTitle = _spawnList[i].title;
			 _spawnList[i]._spawner.Rename();
			 continue;
		  }

		  bool isExistsPrefab = false;

		  for(int a = 0; a < animalsSources.Length; a++)
		  {
			 var comp = animalsSources[a].GetComponent<it.Game.NPC.NPC>();
			 if (comp != null && comp.Uuid == _spawnList[i].uuid)
			 {
				isExistsPrefab = true;
				SpawnPrefab pref = new SpawnPrefab();
				pref.uuid = _spawnList[i].uuid;
				pref._prefab = animalsSources[a];
				_prefabs.Add(pref);
				_spawnList[i].title = comp.Title;
				_spawnList[i]._spawner.AnimalTitle = _spawnList[i].title;
				_spawnList[i]._spawner.Rename();
				break;
			 }

		  }

		  if (!isExistsPrefab)
			 Debug.Log("No exists prefab " + _spawnList[i].uuid);

		}

	 }

#endif
  }
  [System.Serializable]
  public class SpawnInfo
  {
	 public string title;
	 public string uuid;
	 public Vector3 position;
	 public float sqrDistance;
	 public AnimalSpaner _spawner;
	 [HideInInspector]
	 public GameObject _instance;
	 [HideInInspector]
	 public bool isSpawn;
  }
  [System.Serializable]
  public class SpawnPrefab
  {
	 public string uuid;
	 public GameObject _prefab;
  }
}