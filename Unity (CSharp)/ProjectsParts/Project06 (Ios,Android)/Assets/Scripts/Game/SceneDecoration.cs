using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDecoration : MonoBehaviour {

	public string mapName;                        // Название сцены
	public DecorationScene[] decorations;         // Декорации на сцене
	public SpanFloat roadSize;                    // Ширина дорожки
	public Vector2[] points;
	
}

/// <summary>
/// Лекорации сцены
/// </summary>
[System.Serializable]
public class DecorationScene {
	public string objectName;           // Имя Объекта
	public Sprite sprite;               // Спрайт объекта
	public Color spriteColor;               // Спрайт объекта
	public string sortingLayerName;         // Слой отрисовки
	public int sortingOrder;                // Сортировочный слой
	public Texture texture;               // Спрайт объекта
	public Vector3 position;            // Позиция
	public Quaternion rotation;            // Поворот
	public Vector3 scale;               // Размер
}
