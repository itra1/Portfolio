using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapRegion : ScriptableObject {

	public string title;                   // Имя участка карты
	public float distanceStart;           // Дистанция начала учатка
	public float distance;                // Дистанция участка
	public int keys;                    // Количество необходимых ключей
	public RegionType mapType;          // Тип карты
	public float mapDistance;             // Участок на карте
	[HideInInspector]
	public bool isComplited;              // Флаг успешного преодаления дистанции
	public float distanceEnd { get { return distanceStart + distance; } }

}
