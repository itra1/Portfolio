using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct poolObject
{
    public GameObject prefab;
    public int amount;              // Количество, инициализируется в pool
    public float probability;       // Вероятность появления на сцене
    public bool canSpawnAtStart;    // Объект использует при подготовке этапа
}

/* object pooling script
 * We make a list of objects which will be used in the game.
 * Instantiate these objects by the given amount and disactive them
 * 
 * When we need to use certain objects, we simplely active these pooled objects instead of Instantiating on the fly.
 * This allow us to speed up the program, as constantly instantiating and destroying objects are performance expensive.
 */
public class Pool : MonoBehaviour
{
    private List<List<GameObject>> pooledObjects = new List<List<GameObject>>();
    private Dictionary<string,List<GameObject>> pooledNameObjects = new Dictionary<string,List<GameObject>>();


    public void CreatePool(List<KeyValuePair<GameObject, int>> list, Transform parent, List<Transform> listTram = null)
    {
        foreach (KeyValuePair<GameObject, int> entry in list)
        {
            List<GameObject> subPool = new List<GameObject>();

            for (int i = 0; i < entry.Value; i++)
            {
                GameObject obj = (GameObject)Instantiate(entry.Key);

                obj.SetActive(false);

                obj.transform.parent = parent;

                subPool.Add(obj);
                if (listTram != null) listTram.Add(obj.transform);
            }

            pooledObjects.Add(subPool);
        }
    }

    public void CreatePool(Dictionary<string , KeyValuePair<GameObject , int>> list , Transform parent , List<Transform> listTram = null) {
        foreach (string entry in list.Keys) {
            List<GameObject> subPool = new List<GameObject>();

            for (int i = 0 ; i < list[entry].Value ; i++) {
                GameObject obj = (GameObject)Instantiate(list[entry].Key);
                obj.SetActive(false);
                obj.transform.parent = parent;
                subPool.Add(obj);

                if (listTram != null) listTram.Add(obj.transform);
            }

            pooledNameObjects.Add(entry,subPool);
        }
    }
    
    public GameObject getPooledObject(int i , List<Transform> listTram = null , bool canGrow = true) {
        for (int j = 0 ; j < pooledObjects[i].Count ; j++) {
            if (!pooledObjects[i][j].activeInHierarchy)
                return pooledObjects[i][j];
        }

        if (canGrow) {
            GameObject obj = (GameObject)Instantiate(pooledObjects[i][0]);
            obj.transform.parent = pooledObjects[i][0].transform.parent;
            pooledObjects[i].Add(obj);
            if (listTram != null)
                listTram.Add(obj.transform);

            return obj;
        }

        return null;
    }
    
    public GameObject getPooledObject(string name , List<Transform> listTram = null , bool canGrow = true) {
        for (int j = 0 ; j < pooledNameObjects[name].Count ; j++) {
            if (!pooledNameObjects[name][j].activeInHierarchy)
                return pooledNameObjects[name][j];
        }

        if (canGrow) {
            GameObject obj = Instantiate(pooledNameObjects[name][0]);
            obj.transform.parent = pooledNameObjects[name][0].transform.parent;
            pooledNameObjects[name].Add(obj);
            if (listTram != null) listTram.Add(obj.transform);
            return obj;
        }

        return null;
    }

    public void DeactiveAll() {
        for (int i = 0; i < pooledObjects.Count; i++) {
            for (int j = 0; j < pooledObjects[i].Count; j++) {
                pooledObjects[i][j].SetActive(false);
            }
        }
    }

    public void StructProcessor(ref poolObject[] objects
                               ,ref List<KeyValuePair<GameObject, int>> list //список объектов
                               ,ref List<float> cd
                               ,ref List<float> cdAtStart
                                ){
        //Создаем список объектов
        list = new List<KeyValuePair<GameObject, int>>();
        cd = new List<float>();
        cdAtStart = new List<float>();

        float sum1 = 0;
        float sum2 = 0;

        cd.Add(sum1);
        cdAtStart.Add(sum2);

        foreach (poolObject obj in objects)
        {
            //Кроме вычисления распределения, создаем список платформ для процесса пулинга одновременно.
            list.Add(new KeyValuePair<GameObject, int>(obj.prefab, (obj.amount > 0) ? obj.amount : 1));
            sum1 += obj.probability;
            cd.Add(sum1);

            // Распределение на этапе запуска
            sum2 = obj.canSpawnAtStart ? sum2 + obj.probability : sum2;

            cdAtStart.Add(sum2);
        }
        
        if (sum1 != 1f)
            cd = cd.ConvertAll(x => x / sum1); //normalize cd, if it's not already normalized
        
        // Если sum2 расна 0, никакая платформа не имеет преимещество на этапе старта
        if (sum2 == 0f)
            cdAtStart = cd;
        else if (sum2 != 1f)
            cdAtStart = cdAtStart.ConvertAll(x => x / sum2);
    }

    public void StructProcessor(ref GameObject[] objects
                                ,ref List<KeyValuePair<GameObject, int>> list
                                ,int needCount = 1
                                )
    {
        list = new List<KeyValuePair<GameObject, int>>();
        foreach (GameObject obj in objects)
            list.Add(new KeyValuePair<GameObject, int>(obj, needCount));
    }

    public void StructProcessor(GameObject obj, ref List<KeyValuePair<GameObject, int>> list)
    {
        list = new List<KeyValuePair<GameObject, int>>();
        list.Add(new KeyValuePair<GameObject, int>(obj, 1));
    }
}
