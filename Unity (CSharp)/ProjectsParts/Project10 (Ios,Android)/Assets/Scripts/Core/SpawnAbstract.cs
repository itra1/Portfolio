using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GuidForObject))]
public abstract class SpawnAbstract : MonoBehaviourBase {
	
	public SpawnInfo spawnInfo;
}

[System.Serializable]
public class SpawnInfo {
	public SpawnType spawnType;			// Тип генерируемого объекта
	public string title;						// Название генерируемого объекта
#if UNITY_EDITOR
	public Sprite icon;							// Иконка
#endif
}