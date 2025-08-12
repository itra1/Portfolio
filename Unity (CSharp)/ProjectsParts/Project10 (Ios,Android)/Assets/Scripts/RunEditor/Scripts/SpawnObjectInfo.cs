using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectInfo : ScriptableObject {
#if UNITY_EDITOR
	public Sprite icon;														// Иконка
#endif
	public SpawnType spawnType;										// Тип генерируемого объекта
	public string title;													// Название генерируемого объекта
	public MonoBehaviour prefab;									// Префаб
	public List<SpawnObjectParametr> parametrs;   // Параметры
	public Vector3? position;
}

public enum SpawnType {
	none = 0,
	platform = 1,
	coin = 2,
	barrier = 3,
	admin = 4,
  specialBarrier = 5
}

[System.Serializable]
public struct SpawnObjectParametr {
	public string key;
	public string value;
}