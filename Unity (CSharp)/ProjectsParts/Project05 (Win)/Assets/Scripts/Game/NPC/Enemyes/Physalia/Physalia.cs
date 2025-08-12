using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physalia : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
    transform.position += transform.forward * 0.1f * Time.deltaTime;
    }
}
