using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    private void OnCollisionEnter(Collision other) {
        Debug.Log(other.transform.gameObject.name);
    }
}
