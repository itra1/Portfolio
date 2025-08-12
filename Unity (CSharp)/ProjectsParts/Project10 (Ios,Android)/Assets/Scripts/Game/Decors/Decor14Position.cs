using UnityEngine;
using System.Collections;

[System.Serializable]
public struct prefabArray
{
    [Range(0,1)] public float probility;            // Вероятность генерации группы
    public GameObject[] pref;                       // Объекты для генерации
}


public class Decor14Position : MonoBehaviour {

    public prefabArray[] objects;

    void OnEnable()
    {
        foreach (prefabArray one in objects)
            foreach (GameObject gm in one.pref)
                gm.SetActive(false);

        foreach (prefabArray one in objects)
            if(Random.value <= one.probility)
                one.pref[Random.Range(0, one.pref.Length)].SetActive(true);
    }

}
