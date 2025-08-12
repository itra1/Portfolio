using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListPrefabSelect : MonoBehaviour
{
    [SerializeField] private Transform Content;
    [SerializeField] private List<GameObject> Prefabs;

    [Space]
    [HideInInspector] public GameObject PrefabNow;

    public Action<GameObject> callbackOnCreate;
    public Action<GameObject, int> callbackOnCreateList;

    public void CreatePrefabFromList(int n)
    {
        CreatePrefabFromList(n, null);
    }

    public void CreatePrefabFromList(int n, Action<GameObject, int> callback = null)
    {
        if(Prefabs.Count <= n)
        {
            if (PrefabNow != null) Destroy(PrefabNow);
            return;
        }
        CreatePrefab(Prefabs[n]);
        callback?.Invoke(PrefabNow, n);
        callbackOnCreateList?.Invoke(PrefabNow, n);
    }

    public GameObject CreatePrefab(GameObject prefab)
    {
        if (PrefabNow != null) Destroy(PrefabNow);
		PrefabNow = Instantiate(prefab, Content);
        callbackOnCreate?.Invoke(PrefabNow);
        return PrefabNow;
    }

}
