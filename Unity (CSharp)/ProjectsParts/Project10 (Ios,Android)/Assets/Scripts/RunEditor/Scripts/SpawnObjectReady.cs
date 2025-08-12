using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnObjectReady {

	public SpawnObjectInfo spawnObject;
	public Vector3 position;
	public GameObject instance;

  public Vector3 runPosition;

  public SpawnObjectReady GetClone() {

    return new SpawnObjectReady() {
      spawnObject = spawnObject,
      position = position
    };

  }


}
