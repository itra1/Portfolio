using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Environment.Level5.MultiheadArena
{
  public class MultiheadBoxSpawner : MonoBehaviour
  {


#if UNITY_EDITOR
	 [SerializeField] private GameObject _prefab;
	 [SerializeField] private float _spawnRadius = 7;
	 [SerializeField] private float _height = 5;
	 [SerializeField] private LayerMask _layerMask;
	 [SerializeField] private int _countSpawn = 100;
	 [SerializeField] private float _minSize = 1;
	 [SerializeField] private float _maxSize = 6;

	 [ContextMenu("Spawn")]
	 private void Spawn()
	 {
		_prefab.gameObject.SetActive(false);
		Clear();
		StartCoroutine(SpawnerCor());
	 }

	 private IEnumerator SpawnerCor()
	 {
		int counter = 0;
		RaycastHit _hit;
		for (int i = 0; i < _countSpawn; i++)
		{

		  for (int x = 0; x < 10; x++)
		  {
			 Vector3 positionRay = transform.position + Vector3.up * _height + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * Random.Range(0, _spawnRadius);

			 if (Physics.Raycast(positionRay, Vector3.down, out _hit, _height * 1.2f, _layerMask))
			 {
				float scale = Random.Range(_maxSize*0.5f, _maxSize);
				if (_hit.collider.gameObject.name.Contains("Box"))
				{
				  scale = Random.Range(_hit.collider.transform.parent.localScale.x * 0.7f, _hit.collider.transform.parent.localScale.x*0.85f);
				}
				scale = Mathf.Max(scale, _minSize);

				Quaternion targetRotation = Quaternion.Euler(270, Random.Range(0, 360), 0);

				if (!Physics.BoxCast(_hit.point + Vector3.up * _maxSize*2, Vector3.one* scale, Vector3.down, targetRotation, (_maxSize * 2) -scale*1.01f, _layerMask))
				{
				  counter++;
				  
				  GameObject inst = Editor.Instantiate(_prefab, transform);
				  inst.gameObject.SetActive(true);
				  inst.transform.localScale = Vector3.one * scale;
				  inst.transform.position = _hit.point;
				  inst.transform.rotation = targetRotation;
				  Debug.Log(counter);
				  yield return null;
				  break;
				}
			 }
		  }

		}
	 }

	 [ContextMenu("Clear")]
	 private void Clear()
	 {
		while (transform.childCount > 0)
		{
		  DestroyImmediate(transform.GetChild(0).gameObject);
		}
	 }


	 private void OnDrawGizmosSelected()
	 {
		UnityEditor.Handles.DrawSolidDisc(transform.position + Vector3.up * _height, Vector3.up, _spawnRadius);
	 }

#endif

  }
}