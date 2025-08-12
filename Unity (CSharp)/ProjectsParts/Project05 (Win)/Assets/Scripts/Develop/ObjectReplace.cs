using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Develop
{
  public class ObjectReplace : MonoBehaviourBase
  {
    [SerializeField]
    private GameObject[] _objectArray;

    [SerializeField]
    public GameObject _prefab;

    [ContextMenu("Replace")]
    public void Replace()
    {
#if UNITY_EDITOR
      for (int i = 0; i < _objectArray.Length; i++)
      {
        GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(_prefab, _objectArray[i].transform.parent);
        inst.transform.position = _objectArray[i].transform.position;
        inst.transform.rotation = _objectArray[i].transform.rotation;
        inst.transform.localScale = _objectArray[i].transform.localScale;
        DestroyImmediate(_objectArray[i]);
      }
#endif
    }

  }
}