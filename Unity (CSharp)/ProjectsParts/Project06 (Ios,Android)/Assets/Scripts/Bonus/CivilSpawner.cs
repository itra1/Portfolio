using UnityEngine;
using System.Collections;

/// <summary>
/// Генератор гражданских
/// </summary>
public class CivilSpawner : MonoBehaviour {

	[SerializeField] GameObject civilPref;
    float nextCivilTime;

    void Start()
    {
        CalcNextGenTime();
    }

    void Update()
    {
        if(nextCivilTime <= Time.time)
        {
            CalcNextGenTime();
            Instantiate(civilPref, new Vector3(9.21f, Random.Range(-4.01f, -3.98f), 0), Quaternion.identity);
        }
    }

    void CalcNextGenTime()
    {
        nextCivilTime = Time.time+ Random.Range(45f, 70f);
    }
    
}
